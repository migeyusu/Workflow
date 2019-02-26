using System;
using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    /*理论上state之间是多对多的关系，transition是依附于state，为了方便持久化，transition独立出现
     为了节约时间，不实现condition的检查功能*/

    public class Transition : BaseCodeActivity
    {
        public List<TransitionPath> TransitionPaths { get; set; }

        public BaseCodeActivity Trigger { get; set; }

        public Transition()
        {
            TransitionPaths = new List<TransitionPath>();
        }

        internal override IExecuteActivity InternalTranslate(IExecuteActivity executeActivity,
            IDictionary<Guid, IExecuteActivity> stateMapping)
        {
            var endExecuteActivity = executeActivity;
            if (Trigger != null) {
                endExecuteActivity = Trigger.InternalTranslate(executeActivity,stateMapping);
            }

            foreach (var transitionPath in TransitionPaths) {
                transitionPath.InternalTranslate(endExecuteActivity,stateMapping);
            }

            return endExecuteActivity;
        }
    }
}