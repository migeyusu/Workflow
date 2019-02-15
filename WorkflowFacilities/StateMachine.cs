using System.Collections.Generic;

namespace WorkflowFacilities
{
    public abstract class StateMachine
    {
        private PipelineContext pipelineContext = new PipelineContext();
        
        

        /// <summary>
        /// 入口状态
        /// </summary>
        public State EntryState { get; set; }

        public State CurrentState { get; set; }

        /// <summary>
        /// 用于持久化
        /// </summary>
        public List<State> States { get; set; }

        protected StateMachine()
        {
            States = new List<State>();
        }

        public void Run()
        { }

        public void ResumeBookMark()
        { }
    }
}