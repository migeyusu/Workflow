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

        internal override IExecuteActivity InternalTranslate(IExecuteActivity executeActivity)
        {
            var inputExecuteActivity = executeActivity;
            foreach (var customActivity in Activities) {
                inputExecuteActivity = customActivity.InternalTranslate(inputExecuteActivity);
            }

            return inputExecuteActivity as CustomExecuteActivity;
        }
    }
}