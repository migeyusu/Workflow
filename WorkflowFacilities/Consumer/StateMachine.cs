using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{

    /// <summary>
    /// statemachinetemplate的实例，run并保持各种状态
    /// </summary>
    public class StateMachine
    {
        private PipelineContext _pipelineContext = new PipelineContext();
        
        /// <summary>
        /// 开始时表示入口状态
        /// </summary>
        public IExecuteActivity CurrentActivity { get; set; }
        
        public bool IsCompleted { get; set; }

        public State StartState { get; set; }

        public PipelineContext Context {
            get { return _pipelineContext; }
            set { _pipelineContext = value; }
        }
        
        internal StateMachine()
        {
            
        }

        


        public void ResumeBookMark(string name,string value)
        {
                
        }
    }
}