using System;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace ConsoleApp1
{
    public class CodeActivity : BaseCodeActivity
    {
        private readonly Func<PipelineContext, bool> _executeFunc;

        private readonly Action<PipelineContext,string,object> _callbackAction;

        public CodeActivity(Func<PipelineContext, bool> executeFunc, Action<PipelineContext,string,object> callbackAction)
        {
            this._callbackAction = callbackAction;
            _executeFunc = executeFunc;
        }

        public override bool Execute(PipelineContext context)
        {
            return _executeFunc == null || _executeFunc(context);
        }

        public override void BookmarkCallback(PipelineContext context, string bookmarkName, object value)
        {
            _callbackAction?.Invoke(context,bookmarkName,value);
        }
    }
}