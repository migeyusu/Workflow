using System;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace ConsoleApp1
{
    public class CodeActivity : ICustomActivity
    {
        public Guid Version { get; set; }

        public string Name { get; set; }

        public string Bookmark { get; set; }

        private readonly Func<PipelineContext, bool> _executeFunc;

        private readonly Action<PipelineContext> _callbackAction;

        public CodeActivity(Func<PipelineContext, bool> executeFunc, Action<PipelineContext> callbackAction)
        {
            this._callbackAction = callbackAction;
            _executeFunc = executeFunc;
        }

        public bool Execute(PipelineContext context)
        {
            return _executeFunc == null || _executeFunc(context);
        }

        public void BookmarkCallback(PipelineContext context)
        {
            _callbackAction?.Invoke(context);
        }
    }
}