using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Consumer
{
    /// <summary>
    /// state是一类更高级的activity，可视作预设提供的customactivity
    /// </summary>
    public class State
    {
        public BaseCodeActivity Entry { get; set; }

        public BaseCodeActivity Exit { get; set; }

        public List<Transition> Transitions { get; set; }

        public string DisplayName { get; set; }

        public bool IsEndState => this.Transitions.Count == 0;

        public State()
        {
            Transitions = new List<Transition>();
            //Version = Guid.Parse("5DF8A5E9-C16A-4054-9779-CBBE0F128B0D");
        }

        internal void InternalTranslate(IExecuteActivity executeActivity,
            IDictionary<State, IExecuteActivity> mapping)
        {
            var activity = executeActivity;
            var stateEmptyExecuteActivity = new StateSetExecuteActivity {
                DisplayName = this.DisplayName
            };
            mapping.Add(this, stateEmptyExecuteActivity);
            activity.NextActivities.Add(stateEmptyExecuteActivity);
            activity = stateEmptyExecuteActivity;
            activity = Entry != null ? Entry.InternalTranslate(activity) : activity;
            activity = Exit != null ? Exit.InternalTranslate(activity) : activity;
            foreach (var transition in this.Transitions) {
                var endActivity = activity;
                var trigger = transition.Trigger;
                endActivity = trigger != null ? trigger.InternalTranslate(endActivity) : endActivity;
                foreach (var path in transition.TransitionPaths) {
                    var pathConditionFunc = path.ConditionFunc;
                    var pathactivity = endActivity;
                    if (pathConditionFunc != null) {
                        var conditionActivity = new ConditionActivity(pathConditionFunc) {
                            Version = path.Version
                        };
                        pathactivity.NextActivities.Add(conditionActivity);
                        pathactivity = conditionActivity;
                    }

                    var action = path.Aciton;
                    pathactivity = action != null ? action.InternalTranslate(pathactivity) : pathactivity;
                    var pathTo = path.To;
                    if (mapping.TryGetValue(pathTo, out var nextExecuteActivity)) {
                        pathactivity.NextActivities.Add(nextExecuteActivity);
                    }
                    else {
                        pathTo.InternalTranslate(pathactivity, mapping);
                    }
                }
            }
        }
    }
}