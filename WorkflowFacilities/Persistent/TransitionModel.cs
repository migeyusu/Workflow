using System;
using System.Collections.Generic;
using WorkflowFacilities.Consumer;

namespace WorkflowFacilities.Persistent
{
    public class TransitionModel
    {
        public Guid Id { get; set; }
        
        public RunningActivityModel Trigger { get; set; }

        public string Name { get; set; }

        public List<TransitionPathModel> TransitionPaths { get; set; }

        public Guid Version { get; set; }
        
        
    }
}   