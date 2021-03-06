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

        public Guid Id { get; set; }

        public RunningActivityType ActivityType { get; set; }

        public string DisplayName { get; set; }

        public string Bookmark { get; set; }

        public BaseExecuteActivity()
        {
            this.Id = Guid.NewGuid();
            NextActivities = new List<IExecuteActivity>();
        }

        public virtual bool Execute(PipelineContext context)
        {
            return true;
        }

        public virtual void BookmarkCallback(PipelineContext context, string bookmarkName, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsHangUped { get; set; }

        //public IActivity ParentActivity { get; set; }

        public IList<IExecuteActivity> NextActivities { get; set; }
    }
}