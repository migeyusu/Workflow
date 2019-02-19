using System;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Persistent
{
    public class StateModel:IActivity
    {
        public Guid Version { get; set; }

        public string Name { get; set; }

        public RunningActivityModel Entry { get; set; }

        public RunningActivityModel Exit { get; set; }
        
        

    }
}