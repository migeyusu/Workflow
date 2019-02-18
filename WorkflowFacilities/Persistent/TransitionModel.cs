using System;

namespace WorkflowFacilities.Persistent
{
    public class TransitionModel:IActivity
    {
        public Guid Version { get; set; }

        public string Name { get; set; }

        public bool IsHangUped { get; set; }
    }
}   