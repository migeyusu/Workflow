using System;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace ConsoleApp1
{
    public class CodeActivity : ICustomActivity
    {
        public Guid Version { get; set; }

        public string DisplayName { get; set; }

        public string Bookmark { get; set; }

        private readonly Func<PipelineContext, bool> _executeFunc;

        private readonly Action<PipelineContext,string,object> _callbackAction;

        public CodeActivity(Func<PipelineContext, bool> executeFunc, Action<PipelineContext,string,object> callbackAction)
        {
            this._callbackAction = callbackAction;
            _executeFunc = executeFunc;
        }

        public bool Execute(PipelineContext context)
        {
            return _executeFunc == null || _executeFunc(context);
        }

        public void BookmarkCallback(PipelineContext context, string bookmarkName, object value)
        {
            _callbackAction?.Invoke(context,bookmarkName,value);
        }
    }
}