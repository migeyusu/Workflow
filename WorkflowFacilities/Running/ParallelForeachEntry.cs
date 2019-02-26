using WorkflowFacilities.Consumer;

namespace WorkflowFacilities.Running
{
    public class ParallelForeachEntry : CustomExecuteActivity
    {
        public ParallelForeachEntry(ICustomActivity activity) : base(activity)
        {
            this.ActivityType = RunningActivityType.ParallelForeachEnty;
            this.DisplayName = RunningActivityType.ParallelForeachEnty.ToString();
        }
    }
}