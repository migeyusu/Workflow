using System;

namespace WorkflowFacilities.Consumer
{
    public class TransitionPath
    {
        public State To { get; set; }

        public ICustomActivity Aciton { get; set; }

        public Func<bool> ConditionFunc;
    }
}