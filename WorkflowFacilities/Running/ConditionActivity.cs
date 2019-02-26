using System;
using WorkflowFacilities.Consumer;

namespace WorkflowFacilities.Running
{
    public class ConditionActivity:CustomExecuteActivity
    {
        public const string Conditionstring = "Condition";

        public ConditionActivity(ICustomActivity activity):base(activity)
        {
            this.ActivityType = RunningActivityType.Condition;
            this.DisplayName = Conditionstring;
            //this.Version = Guid.Parse("5252E0F4-407B-4970-BA10-8D157E3E8BBD");
        }
    }
}