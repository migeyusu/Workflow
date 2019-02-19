using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    public  class StateMachineTemplate
    {
        public List<State> States { get; set; }

        public List<Transition> Transitions { get; set; }

        public List<TransitionPath> TransitionPaths { get; set; }
        
        public State StartState { get; set; }    
        
        public StateMachine Create()
        {
            var stateMachine = new StateMachine(){StartState = StartState};
            return stateMachine;
        }
    }
}