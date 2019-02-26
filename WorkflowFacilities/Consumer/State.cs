using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    /// <summary>
    /// state是一类更高级的activity，可视作预设提供的customactivity
    /// </summary>
    public class State:BaseCodeActivity
    {
        public BaseCodeActivity Entry { get; set; }

        public BaseCodeActivity Exit { get; set; }

        public List<Transition> Transitions { get; set; }
        
        public bool IsEndState => this.Transitions.Count == 0;

        public State()
        {
            Transitions = new List<Transition>();
            //Version = Guid.Parse("5DF8A5E9-C16A-4054-9779-CBBE0F128B0D");
        }

        internal override IExecuteActivity InternalTranslate(IExecuteActivity executeActivity,
            IDictionary<Guid, IExecuteActivity> mapping)
        {
            var activity = executeActivity;
            var stateEmptyExecuteActivity = new StateSetExecuteActivity {
                DisplayName = this.DisplayName
            };
            mapping.Add(this.Version, stateEmptyExecuteActivity);
            activity.NextActivities.Add(stateEmptyExecuteActivity);
            activity = stateEmptyExecuteActivity;
            activity = Entry != null ? Entry.InternalTranslate(activity,mapping) : activity;
            activity = Exit != null ? Exit.InternalTranslate(activity,mapping) : activity;
            foreach (var transition in this.Transitions) {
                transition.InternalTranslate(activity, mapping);
            }

            return null;
        }
    }
}