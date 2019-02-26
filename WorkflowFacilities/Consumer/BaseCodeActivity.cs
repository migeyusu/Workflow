using System;
using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    public class BaseCodeActivity : ICustomActivity
    {
        public Guid Version { get; set; }

        public string DisplayName { get; set; }

        public virtual bool Execute(PipelineContext context)
        {
            return true;
        }

        public virtual void BookmarkCallback(PipelineContext context, string bookmarkName, object value)
        { }

        /// <summary>
        /// must return last CustomExecuteActivity
        /// </summary>
        /// <param name="executeActivity">input chain</param>
        /// <param name="stateMapping"></param>
        /// <returns></returns>
        internal virtual IExecuteActivity InternalTranslate(IExecuteActivity executeActivity,
            IDictionary<Guid, IExecuteActivity> stateMapping)
        {
            var customExecuteActivity = new CustomExecuteActivity(this);
            executeActivity.NextActivities.Add(customExecuteActivity);
            return customExecuteActivity;
        }
    }
}