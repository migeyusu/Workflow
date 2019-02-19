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

        public RunningActivityType ActivityType { get; set; }
        
        public string Name { get; set; }

        public string Bookmark { get; set; }

        public BaseExecuteActivity()
        {
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

        //public IActivity ParentActivity { get; set; }

        public IList<IExecuteActivity> NextActivities { get; set; }
    }

    public enum RunningActivityType
    {
        Condition,
        Custom,
        Empty,
        Start,
    } 
}