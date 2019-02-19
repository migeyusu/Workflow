using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using WorkflowFacilities.Persistent;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    /// <summary>
    /// statemachinetemplate的实例，run并保持各种状态
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// 运行activity链的起点
        /// </summary>
        public IExecuteActivity RunningExecuteActivityEntry { get; set; }

        public Guid Id { get; set; }

        /// <summary>
        /// state的起点
        /// </summary>
        public State StartState => Template.StartState;

        public StateMachineTemplate Template { get; set; }

        public PipelineContext Context { get; set; }

        internal StateMachine()
        {
            this.Context = new PipelineContext();
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// 映射到statemachine，第一次生成
        /// </summary>
        /// <returns></returns>
        internal StateMachineModel CreateStateMachineModel()
        {
            if (RunningExecuteActivityEntry == null) {
                //throw new NotSupportedException("should translate first");
                var stateMachineScheduler = new StateMachineScheduler();
                stateMachineScheduler.Translate(this);
            }

            var allActivityModels = new Dictionary<Guid, RunningActivityModel>();
            var startActivityModel = new RunningActivityModel();
            Decorate(RunningExecuteActivityEntry, startActivityModel, allActivityModels);
            var stateMachineModel = new StateMachineModel {
                StartActivityModel = startActivityModel,
                Id = this.Id,
                StateMachineTemplateVersion = Template.Version,
            };
            return stateMachineModel;
        }

        /// <summary>
        /// 将运行后的变化持久化到数据库
        /// </summary>
        /// <param name="stateMachineModel"></param>
        internal void Store(StateMachineModel stateMachineModel)
        {
            stateMachineModel.CurrentStateName = this.Context.CurrentStateName;
            stateMachineModel.IsCompleted = this.Context.IsCompleted;
            var dictionary = this.Context.LocalVariableDictionary;
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream()) {
             
                binaryFormatter.Serialize(memoryStream,dictionary);
                var s = Encoding.Unicode.GetString(memoryStream.ToArray());   

            }
            
            //stateMachineModel.LocalVariousDictionary
        }

        internal StateMachineModel Get()
        {
            throw new NotImplementedException();
        }
        

        private void Decorate(IExecuteActivity executeActivity, RunningActivityModel activityModel,
            Dictionary<Guid, RunningActivityModel> cache)
        {
            cache.Add(activityModel.Id, activityModel);
            activityModel.Version = executeActivity.Version;
            activityModel.Name = executeActivity.Name;
            activityModel.Bookmark = executeActivity.Bookmark;
            foreach (var nextActivity in executeActivity.NextActivities) {
                var runningActivityModel = new RunningActivityModel();
                Decorate(nextActivity, runningActivityModel, cache);
                activityModel.RunningActivityModels.Add(runningActivityModel);
            }
        }
    }
}