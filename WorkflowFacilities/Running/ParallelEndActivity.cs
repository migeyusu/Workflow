namespace WorkflowFacilities.Running {
    public class ParallelEndActivity : BaseExecuteActivity
    {
        private const string ParallelEndString = "ParallelEnd";
        public ParallelEndActivity()
        {
            this.ActivityType = RunningActivityType.ParallelEnd;
            this.DisplayName = ParallelEndString;
        }

        public override bool Execute(PipelineContext context)
        {
            var name = Version.ToString();
            var count = int.Parse(context.Get(name));
            if (count == 1)
            {
                context.Remove(name);
                return true;
            }
            else
            {
                count--;
                context.Set(name, count.ToString());
                return false;
            }
        }
    }
}