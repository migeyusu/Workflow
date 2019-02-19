using System;

namespace WorkflowFacilities.Persistent
{
    public class TransitionPathModel
    {
        public Guid Id { get; set; }

        public RunningActivityModel Action { get; set; }
    }
}