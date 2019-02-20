using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Persistent
{
    public class Field:IDisposable
    {
        private readonly WorkflowDbContext _workflowDbContext;

        public Field(WorkflowDbContext workflowDbContext)
        {
            this._workflowDbContext = workflowDbContext;
        }
        
        /// <summary>
        /// 检查模板是否有更新
        /// </summary>
        public void CheckUpdates()
        {
            var guids = _workflowDbContext.StateMachineTemplateModels
                .Select(model => model.Version);
            var stateMachineTemplates = WorkflowFact.Templates.Where(template => !guids.Contains(template.Version));
            
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">statemachine id</param>
        /// <returns></returns>
        public StateMachine Get(Guid id)
        {
            var stateMachineModel = _workflowDbContext.StateMachines.Find(id);
            if (stateMachineModel==null) {
                throw new ObjectNotFoundException("指定的工作流无法查询到！");
            }

            var stateMachineTemplate = WorkflowFact.Get(stateMachineModel.Id);
            if (stateMachineTemplate==null) {
                throw new VersionNotFoundException("该工作流关联的模板无法查询到！");
            }
            var pipelineContext = new PipelineContext() {
                CurrentStateName = stateMachineModel.CurrentStateName,
                IsCompleted = stateMachineModel.IsCompleted,   
            };
            var binaryFormatter = new BinaryFormatter();
            var bytes = Encoding.Unicode.GetBytes(stateMachineModel.LocalVariousDictionary);
            using (var memoryStream = new MemoryStream(bytes)) {
                var dictionary = binaryFormatter.Deserialize(memoryStream) as ConcurrentDictionary<string, string>;
                pipelineContext.LocalVariableDictionary = dictionary;
            }


            var allExecuteActivities = new Dictionary<Guid, IExecuteActivity>();
            var executeActivity = WorkflowFact.Deserialize(stateMachineModel.StartActivityModel,allExecuteActivities,stateMachineTemplate);
            var stateMachine = new StateMachine {
                Id = stateMachineModel.Id,
                Template = stateMachineTemplate,
                ExecuteActivityChainEntry = executeActivity,
                Context = pipelineContext,
                Name = stateMachineModel.Name
            };   

            return stateMachine;
        }
        
        public void Save()
        {
            _workflowDbContext.SaveChanges();
        }

        public void Dispose()
        {
            _workflowDbContext.Dispose();
        }
    }
}