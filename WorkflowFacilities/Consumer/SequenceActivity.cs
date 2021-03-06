using System;
using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    public class SequenceActivity : BaseCodeActivity
    {
        public List<BaseCodeActivity> Activities { get; set; }

        public SequenceActivity()
        {
            Activities = new List<BaseCodeActivity>();
        }

        internal override IExecuteActivity InternalTranslate(IExecuteActivity executeActivity,
            IDictionary<Guid, IExecuteActivity> stateMapping)
        {
            var inputExecuteActivity = executeActivity;
            foreach (var customActivity in Activities) {
                inputExecuteActivity = customActivity.InternalTranslate(inputExecuteActivity,stateMapping);
            }

            return inputExecuteActivity as CustomExecuteActivity;
        }
    }
}