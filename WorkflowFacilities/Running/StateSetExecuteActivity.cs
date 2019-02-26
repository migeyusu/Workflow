using System;

namespace WorkflowFacilities.Running
{
    /// <summary>
    /// state的空activity，用于填充空state，每次state执行前执行，表示已经进入该state
    /// </summary>
    public class StateSetExecuteActivity : BaseExecuteActivity
    {
        public StateSetExecuteActivity() : base()
        {
            this.ActivityType = RunningActivityType.Set;
            //this.Version = Guid.Parse("B4B4C3B6-D102-4F7E-B8B9-0367244EFF3B");
        }

        public override bool Execute(PipelineContext context)
        {
            //在不同的state切换时需要清空bookmark
            context.SuspendedActivities.Clear();
            context.CurrentStateName = this.DisplayName;
            return true;
        }
    }
}