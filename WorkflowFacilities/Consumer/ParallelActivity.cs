using System;
using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    public class ParallelActivity : BaseCodeActivity
    {
        public List<BaseCodeActivity> Activities { get; set; }

        internal override IExecuteActivity InternalTranslate(IExecuteActivity executeActivity,
            IDictionary<Guid, IExecuteActivity> stateMapping)
        {
            var sync = Guid.NewGuid();
            //并行同步，本质上是过滤器，等全部都执行完后放行
            var parallelEndActivity = new ParallelEndActivity() {Version = sync};
            var parallelStartActivity = new ParallelStartActivity() {Version = sync};
            executeActivity.NextActivities.Add(parallelStartActivity);
            foreach (var activity in Activities) {
                var customExecuteActivity = new CustomExecuteActivity(activity);
                parallelStartActivity.NextActivities.Add(customExecuteActivity);
                customExecuteActivity.NextActivities.Add(parallelEndActivity);
            }

            return parallelEndActivity;
        }
    }

}