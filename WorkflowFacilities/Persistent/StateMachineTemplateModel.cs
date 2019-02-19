using System;
using System.Collections.Generic;

namespace WorkflowFacilities.Persistent
{
    public class StateMachineTemplateModel
    {
        public Guid Id { get; set; }

        public Guid Version { get; set; }

        public List<StateModel> StateModels { get; set; }
    }
}