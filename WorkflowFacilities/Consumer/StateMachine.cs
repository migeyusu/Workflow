using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    public abstract class StateMachine
    {
        private PipelineContext pipelineContext = new PipelineContext();
        
        /// <summary>
        /// 开始时表示入口状态
        /// </summary>
        public State CurrentState { get; set; }

        public bool IsCompleted { get; set; }

        protected StateMachine()
        {
            
        }

        public void Run()
        {
            if (CurrentState.Entry.IsHangUped) {
                
            }
        }

        private void InternalRun(State state)
        {
            
            if (state.IsEndState) {
                IsCompleted = true;
            }
            if (state.Entry.IsHangUped) {
                return;
            }

            if (state.Exit.IsHangUped) {
                return;
            }
        }

        public void ResumeBookMark(string name,string value)
        {
                
        }
    }
}