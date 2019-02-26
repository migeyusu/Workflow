using System;
using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    /*
     * 允许非继承新建使用delegation
     */

    /// <summary>
    /// empty transitionpath doesn't need to add into workflow transition collection
    /// which has empty conditionfunc
    /// </summary>
    public class TransitionPath : BaseCodeActivity
    {
        public State To { get; set; }

        public BaseCodeActivity Action { get; set; }

        private readonly Func<PipelineContext, bool> _conditionFunc;

        public TransitionPath(Func<PipelineContext, bool> conditionFunc=null)
        {
            _conditionFunc = conditionFunc;
        }

        public override bool Execute(PipelineContext context)
        {
            return _conditionFunc.Invoke(context);
        }

        internal override IExecuteActivity InternalTranslate(IExecuteActivity executeActivity,
            IDictionary<Guid, IExecuteActivity> stateMapping)
        {
            if (_conditionFunc == null && this.Action == null) {
                return executeActivity;
            }

            var endExecuteActivity = executeActivity;
            if (_conditionFunc != null) {
                var conditionActivity = new ConditionActivity(this);
                endExecuteActivity.NextActivities.Add(conditionActivity);
            }

            if (this.Action != null) {
                endExecuteActivity = this.Action.InternalTranslate(endExecuteActivity,stateMapping);
            }

            if (stateMapping.TryGetValue(To.Version,out var nextExecuteActivity)) {
                endExecuteActivity.NextActivities.Add(nextExecuteActivity);
            }
            else {
                To.InternalTranslate(endExecuteActivity, stateMapping);
            }
            return endExecuteActivity; 
        }
    }
}