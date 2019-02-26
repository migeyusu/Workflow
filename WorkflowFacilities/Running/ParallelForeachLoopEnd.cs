namespace WorkflowFacilities.Running
{
    public class ParallelForeachLoopEnd : BaseExecuteActivity
    {
        public ParallelForeachLoopEnd()
        {
            this.ActivityType = RunningActivityType.ParallelForeachLoopEnd;
            this.DisplayName = RunningActivityType.ParallelForeachLoopEnd.ToString();
        }
    }
}