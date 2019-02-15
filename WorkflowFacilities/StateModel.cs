using System;

namespace WorkflowFacilities
{
    public class StateModel:IActivity
    {
        public Guid Version { get; set; }

        public ActivityModel Entry { get; set; }

        public ActivityModel Exit { get; set; }

        public Transition Type { get; set; }

    }
}