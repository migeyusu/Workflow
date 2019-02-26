namespace WorkflowFacilities.Running {
    /// <summary>
    /// 控制导向
    /// </summary>
    public class ParallelStartActivity : BaseExecuteActivity
    {
        private const string ParallelStartString = "ParallelStart";
        
        public ParallelStartActivity()
        {
            this.ActivityType = RunningActivityType.ParallelStart;
            this.DisplayName = ParallelStartString;
        }

        public override bool Execute(PipelineContext context)
        {
            context.Set(Version.ToString(), this.NextActivities.Count.ToString());
            return true;
        }
    }
}