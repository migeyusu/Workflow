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
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid Version { get; set; }
        
        /// <summary>
        /// activity链
        /// </summary>
        public IExecuteActivity ExecuteActivityChainEntry { get; set; }

        public bool IsCompleted => Context.IsCompleted;

        public bool IsRunning => Context.IsRunning;

        public PipelineContext Context { get; set; }

        internal StateMachine()
        { }


    }
}