using System;
using System.Collections.Generic;

namespace WorkflowFacilities.Running
{
    /*
     * 和自定义activity平级
     */
    public class BaseExecuteActivity : IExecuteActivity
    {
        public Guid Version { get; set; }

        public string Name { get; set; }

        public string Bookmark { get; set; }

        public BaseExecuteActivity()
        {
            this.Version = Guid.Parse("22B34E74-3F49-4F3A-AE00-AE7BA4768D08");
            this.Name = "Base";
            NextActivities = new List<IExecuteActivity>();
        }

        public virtual bool Execute(PipelineContext context)
        {
            return true;
        }

        public virtual void BookmarkCallback(PipelineContext context, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsHangUped { get; set; }

        public bool Executed { get; set; }

        public IActivity ParentActivity { get; set; }

        public IList<IExecuteActivity> NextActivities { get; set; }
    }

    /// <summary>
    /// state的空activity，用于填充空state，每次state执行前执行，表示已经进入该state
    /// </summary>
    public class StateEmptyExecuteActivity : BaseExecuteActivity
    {
        public StateEmptyExecuteActivity() : base()
        {
            this.Version = Guid.Parse("B4B4C3B6-D102-4F7E-B8B9-0367244EFF3B");
            this.Name = "State";
        }
    }


    /// <summary>
    /// 用于包裹自定义执行器
    /// </summary>
    public class CustomExecuteActivity : BaseExecuteActivity
    {
        private readonly Func<PipelineContext, bool> _executeFunc;

        private readonly Action<PipelineContext, object> _callbackAction;
        
        public CustomExecuteActivity(Func<PipelineContext, bool> executeFunc,
            Action<PipelineContext, object> callbackAction)
        {
            this._executeFunc = executeFunc;
            this._callbackAction = callbackAction;
        }
        
        public override bool Execute(PipelineContext context)
        {
            return _executeFunc == null || _executeFunc(context);
        }

        public override void BookmarkCallback(PipelineContext context, object value)
        {
            _callbackAction?.Invoke(context, value);
        }
    }
}