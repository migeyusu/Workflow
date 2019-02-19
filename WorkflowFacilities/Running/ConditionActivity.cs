using System;

namespace WorkflowFacilities.Running
{
    public class ConditionActivity:BaseExecuteActivity
    {
        private readonly Func<PipelineContext,bool> _satisfy;
        
        public ConditionActivity(Func<PipelineContext,bool> satisfy):base()
        {
            this._satisfy = satisfy;
            this.ActivityType = RunningActivityType.Condition;
            //this.Version = Guid.Parse("5252E0F4-407B-4970-BA10-8D157E3E8BBD");
        }

        public override bool Execute(PipelineContext context)
        {
            return _satisfy(context);
        }
    }
}