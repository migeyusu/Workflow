using System;

namespace WorkflowFacilities.Running
{
    /// <summary>
    /// 用于包裹自定义执行器
    /// </summary>
    public class CustomExecuteActivity : BaseExecuteActivity
    {
        private readonly Func<PipelineContext, bool> _executeFunc;

        private readonly Action<PipelineContext> _callbackAction;
        
        public CustomExecuteActivity(Func<PipelineContext, bool> executeFunc,
            Action<PipelineContext> callbackAction):base()
        {
            this._executeFunc = executeFunc;
            this._callbackAction = callbackAction;
            this.Version = Guid.Parse("D9D8EA85-AD19-476D-8FD3-1E3AA47B01B3");
        }
        
        public override bool Execute(PipelineContext context)
        {
            return _executeFunc == null || _executeFunc(context);
        }

        public override void BookmarkCallback(PipelineContext context)
        {
            _callbackAction?.Invoke(context);
        }
    }
}