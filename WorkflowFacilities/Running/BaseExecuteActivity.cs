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

        public virtual void BookmarkCallback(PipelineContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsHangUped { get; set; }

        public bool Executed { get; set; }

        public IActivity ParentActivity { get; set; }

        public IList<IExecuteActivity> NextActivities { get; set; }
    }
}