using System;
using System.Collections.Generic;

namespace WorkflowFacilities.Persistent
{
    /*
     * 暂时不用
     */
    public class StateMachineTemplateModel
    {
        public Guid Version { get; set; }

        public List<StateModel> StateModels { get; set; }

        public List<TransitionModel> TransitionModels { get; set; }

        public List<TransitionPathModel> TransitionPathModels { get; set; }

        public StateModel StartStateModel { get; set; }
    }
}