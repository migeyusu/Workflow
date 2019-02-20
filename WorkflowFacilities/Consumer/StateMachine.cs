using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public string Name { get; set; }

        /// <summary>
        /// activity链
        /// </summary>
        public IExecuteActivity ExecuteActivityChainEntry { get; set; }

        public bool IsCompleted => Context.IsCompleted;

        public bool IsRunning => Context.IsRunning;

        public Guid Id { get; set; }

        public StateMachineTemplate Template { get; set; }

        public PipelineContext Context { get; set; }

        /// <summary>
        /// 映射到statemachine，第一次生成
        /// </summary>
        /// <returns></returns>
        internal StateMachineModel CreateStateMachineModel()
        {
            if (ExecuteActivityChainEntry == null) {
                //throw new NotSupportedException("should translate first");
                var stateMachineScheduler = new StateMachineScheduler();
                stateMachineScheduler.Translate(this);
            }

            var allActivityModels = new Dictionary<Guid, RunningActivityModel>();
            var startActivityModel = WorkflowFact.Serialize(ExecuteActivityChainEntry,allActivityModels);
            var stateMachineModel = new StateMachineModel {
                StartActivityModel = startActivityModel,
                Id = this.Id,
                StateMachineTemplateVersion = Template.Version,
                RunningActivityModels = allActivityModels.Values.ToList(),
                Name = this.Name,
            };
            return stateMachineModel;
        }

        /// <summary>
        /// 将运行后的变化映射
        /// </summary>
        /// <param name="stateMachineModel"></param>
        internal void UpdateMap(StateMachineModel stateMachineModel)
        {
            stateMachineModel.CurrentStateName = this.Context.CurrentStateName;
            stateMachineModel.IsCompleted = this.Context.IsCompleted;
            var dictionary = this.Context.LocalVariableDictionary;
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream()) {
                binaryFormatter.Serialize(memoryStream, dictionary);
                stateMachineModel.LocalVariousDictionary = Encoding.Unicode.GetString(memoryStream.ToArray());
            }

            stateMachineModel.SuspendedActivityModels.Clear();
            var runningActivityModels = stateMachineModel.RunningActivityModels;
            var @select = this.Context.SuspendedActivities.Values
                .Select(activity => runningActivityModels.First(model => model.Id == activity.Id));
            stateMachineModel.SuspendedActivityModels.AddRange(select);
        }
    }
}