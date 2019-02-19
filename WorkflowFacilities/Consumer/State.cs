using System;
using System.Collections.Generic;

namespace WorkflowFacilities.Consumer
{
    /// <summary>
    /// state是一类更高级的activity，可视作预设提供的customactivity
    /// </summary>
    public class State
    {
        public ICustomActivity Entry { get; set; }

        public ICustomActivity Exit { get; set; }

        public List<Transition> Transitions { get; set; }
        
        public string Name { get; set; }

        public bool IsEndState => this.Transitions.Count == 0;

        public State()
        {
            Transitions = new List<Transition>();
            //Version = Guid.Parse("5DF8A5E9-C16A-4054-9779-CBBE0F128B0D");
        }
    }
}