
using System;
using System.Collections.Generic;

namespace WorkflowFacilities
{
    public class State:IActivity
    {
        public ICustomActivity Entry { get; set; }

        public ICustomActivity Exit { get; set; }

        public List<Transition> Transitions { get; set; }

        public Guid Version { get; set; }

        public string Name { get; set; }

        public bool IsEndState => this.Transitions.Count == 0;

        public State()
        {
            Transitions=new List<Transition>();
        }
    }
}