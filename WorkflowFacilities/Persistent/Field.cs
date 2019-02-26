using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Persistent
{
    public class Field : IDisposable
    {
        private readonly WorkflowDbContext _workflowDbContext;

        public Field(WorkflowDbContext workflowDbContext)
        {
            this._workflowDbContext = workflowDbContext;
        }

        /// <summary>
        /// 检查模板更新
        /// </summary>
        public void CheckUpdates()
        {
            var guids = _workflowDbContext.StateMachineTemplateModels
                .Select(model => model.Version);
            var runningActivityModels = new Dictionary<Guid, RunningActivityModel>();
            var insertTemplateModels = WorkflowFact.AllTemplateTypes.Values
                .Select(type => Activator.CreateInstance(type) as StateMachineTemplate)
                .Where(template => template != null && !guids.Contains(template.Version))
                .Select(template => {
                    template.Generation();
                    var startActivity = StateMachineScheduler.Translate(template);
                    return new StateMachineTemplateModel {
                        Version = template.Version,
                        CodeTemplateName = template.Name,
                        StartActivityModel = StateMachineScheduler
                            .Serialize(startActivity, runningActivityModels),
                        RunningActivityModels = runningActivityModels.Values.ToList()
                    };
                });
            _workflowDbContext.ActivityModels.AddRange(runningActivityModels.Values);
            _workflowDbContext.StateMachineTemplateModels.AddRange(insertTemplateModels);
        }

        public StateMachine NewStateMachine<T>() where T : StateMachineTemplate
        {
            var template = Activator.CreateInstance<T>() as StateMachineTemplate;
            //虽然已经生成了activity模板对象集合，但是不能直接用于生成运行链，因为每次id会不同
            template.Generation();
            var templateModel = _workflowDbContext.StateMachineTemplateModels.Find(template.Version);
            if (templateModel == null) {
                throw new ObjectNotFoundException("找不到工作流关联的模板，请先更新模板");
            }

            var activityDictionary = new Dictionary<Guid, IExecuteActivity>();
            var executeActivity =
                StateMachineScheduler.Deserialize(templateModel.StartActivityModel, activityDictionary, template);
            var stateMachine = new StateMachine() {
                Version = templateModel.Version,
                Id = Guid.NewGuid(),
                Context = new PipelineContext(),
                TemplateName = template.Name,
                ExecuteActivityChainEntry = executeActivity,
            };
            return stateMachine;
        }

        public void Save(StateMachine stateMachine)
        {
            var machineModel = _workflowDbContext.StateMachineModels.Find(stateMachine.Id);
            if (machineModel != null) {
                throw new DuplicateNameException("该statemachine已持久化，不能重复保存！");
            }

            var stateMachineTemplateModel =
                _workflowDbContext.StateMachineTemplateModels.Find(stateMachine.Version);
            if (stateMachineTemplateModel == null) {
                throw new ObjectNotFoundException($"statemachine{stateMachine.TemplateName}无法查询到指定的模板");
            }

            var stateMachineContext = stateMachine.Context;
            var stateMachineModel = new StateMachineModel {
                Id = stateMachine.Id,
                Name = stateMachine.TemplateName,
                TemplateModel = stateMachineTemplateModel,
                CurrentStateName = stateMachineContext.CurrentStateName,
                IsCompleted = stateMachineContext.IsCompleted,
                IsRunning = stateMachineContext.IsRunning,
            };
            var dictionary = stateMachineContext.PersistableLocals;
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream()) {
                binaryFormatter.Serialize(memoryStream, dictionary);
                stateMachineModel.LocalVariousDictionary = memoryStream.ToArray();
            }

            var @select = stateMachineContext.SuspendedActivities
                .Select(keyvaluepair => new SuspendedRunningActivityModel() {
                    BookmarkName = keyvaluepair.Key,
                    RunningActivityModel =
                        stateMachineTemplateModel.RunningActivityModels.First(
                            model => model.Id == keyvaluepair.Value.Id)
                }).ToArray();
            if (@select.Any()) {
                _workflowDbContext.SuspendedRunningActivityModels.AddRange(select);
                stateMachineModel.SuspendedRunningActivityModels.AddRange(select);
            }
            _workflowDbContext.StateMachineModels.Add(stateMachineModel);
        }

        public void Update(StateMachine stateMachine)
        {
            var stateMachineModel = _workflowDbContext.StateMachineModels.Find(stateMachine.Id);
            if (stateMachineModel == null) {
                throw new KeyNotFoundException("数据库中找不到指定的statemachine实例！请先存入数据库。");
            }

            var stateMachineContext = stateMachine.Context;
            stateMachineModel.CurrentStateName = stateMachineContext.CurrentStateName;
            stateMachineModel.IsCompleted = stateMachineContext.IsCompleted;
            stateMachineModel.IsRunning = stateMachineContext.IsRunning;
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream()) {
                binaryFormatter.Serialize(memoryStream, stateMachineContext.PersistableLocals);
                stateMachineModel.LocalVariousDictionary = memoryStream.ToArray();
            }

            stateMachineModel.SuspendedRunningActivityModels.Clear();
            var stateMachineTemplateModel = stateMachineModel.TemplateModel;
            var suspendedActivityModels = stateMachineContext.SuspendedActivities
                .Select(pair => new SuspendedRunningActivityModel() {
                    BookmarkName = pair.Key,
                    RunningActivityModel =
                        stateMachineTemplateModel.RunningActivityModels.First(model => model.Id == pair.Value.Id)
                }).ToArray();
            _workflowDbContext.SuspendedRunningActivityModels.AddRange(suspendedActivityModels);
            stateMachineModel.SuspendedRunningActivityModels.AddRange(suspendedActivityModels);
        }

        public void Delete(StateMachine stateMachine)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">statemachine id</param>
        /// <returns></returns>
        public StateMachine Get(Guid id)
        {
            var stateMachineModel = _workflowDbContext.StateMachineModels.Find(id);
            if (stateMachineModel == null) {
                throw new ObjectNotFoundException("指定的工作流无法查询到！");
            }

            var templateModel = stateMachineModel.TemplateModel;
            if (!WorkflowFact.AllTemplateTypes.TryGetValue(templateModel.CodeTemplateName, out var type)) {
                throw new KeyNotFoundException($"找不到名为{templateModel.CodeTemplateName}的模板类！");
            }

            var stateMachineTemplate =
                Activator.CreateInstance(type ?? throw new TypeLoadException("数据库关联模板的代码已经变更，未找到指定类型")) as
                    StateMachineTemplate;
            if (stateMachineTemplate == null) {
                throw new InstanceNotFoundException("未能创建指定的模板！请检查代码文件是否有缺失");
            }

            stateMachineTemplate.Generation();
            var executeActivities = new Dictionary<Guid, IExecuteActivity>();
            var activity = StateMachineScheduler.Deserialize(templateModel.StartActivityModel, executeActivities,
                stateMachineTemplate);
            var activities = stateMachineModel.SuspendedRunningActivityModels
                .ToDictionary(model => model.BookmarkName, model => executeActivities[model.RunningActivityModel.Id]);
            var pipelineContext = new PipelineContext() {
                CurrentStateName = stateMachineModel.CurrentStateName,
                IsCompleted = stateMachineModel.IsCompleted,
                IsRunning = stateMachineModel.IsRunning,
                SuspendedActivities = activities
            };
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(stateMachineModel.LocalVariousDictionary)) {
                var dictionary = binaryFormatter.Deserialize(memoryStream) as ConcurrentDictionary<string, string>;
                pipelineContext.PersistableLocals = dictionary;
            }

            var stateMachine = new StateMachine() {
                Id = stateMachineModel.Id,
                ExecuteActivityChainEntry = activity,
                Context = pipelineContext,
                TemplateName = stateMachineModel.Name
            };

            return stateMachine;
        }

        public void SaveChanges()
        {
            _workflowDbContext.SaveChanges();
        }

        public void Dispose()
        {
            _workflowDbContext.Dispose();
        }
    }
}