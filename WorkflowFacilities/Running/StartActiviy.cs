using System;

namespace WorkflowFacilities.Running
{
    public class StartActiviy : BaseExecuteActivity
    {
        public const string StartString = "Start";

        public StartActiviy() : base()
        {
            this.ActivityType = RunningActivityType.Start;
            Id = Guid.NewGuid();
            this.Name = StartString;
            //this.Version = Guid.Parse("5DF8A5E9-C16A-4054-9779-CBBE0F128B0D");
        }
    }
}