using System;
using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    public abstract class StateMachineTemplate
    {
        public Guid Version { get; set; }

        public List<State> States { get; set; }

        public List<Transition> Transitions { get; set; }

        public List<TransitionPath> TransitionPaths { get; set; }

        public List<ICustomActivity> CustomActivities { get; set; }

        public State StartState { get; set; }

        protected StateMachineTemplate()
        {
            this.States = new List<State>();
            this.Transitions = new List<Transition>();
            this.TransitionPaths = new List<TransitionPath>();
            this.CustomActivities = new List<ICustomActivity>();
        }

        /// <summary>
        /// generation
        /// </summary>
        public abstract void OnGeneration();

        /// <summary>
        /// new instance initialize
        /// </summary>
        public abstract void OnInitialize(PipelineContext pipelineContext);

        public StateMachine GetStateMachine()
        {
            var stateMachine = new StateMachine() {
                Template = this
            };
            return stateMachine;
        }
    }
}