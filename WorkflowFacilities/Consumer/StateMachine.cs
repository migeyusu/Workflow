using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    /// <summary>
    /// statemachinetemplate的实例，run并保持各种状态
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// 开始时表示入口状态
        /// </summary>
        public IExecuteActivity CurrentActivity { get; set; }

        public State StartState { get; set; }

        public PipelineContext Context { get; set; } = new PipelineContext();

        internal StateMachine()
        { }

    }
}