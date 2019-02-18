using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace WorkflowFacilities
{
    public class StateMachineScheduler
    {
        /*
         * 翻译成可执行的activity
         */
        public void Run(StateMachine stateMachine) { }

        /// <summary>
        /// 状态列表，映射入口activity
        /// </summary>
        private Dictionary<State, IExecuteActivity> _activitiesDictionary;
        
        private void Translate(State state, IExecuteActivity executeActivity)
        {
            var activity = executeActivity;
            var stateEmptyExecuteActivity = new StateEmptyExecuteActivity {ParentActivity = state};
            _activitiesDictionary.Add(state, stateEmptyExecuteActivity);
            activity.NextActivities.Add(stateEmptyExecuteActivity);
            activity = stateEmptyExecuteActivity;
            var entry = state.Entry;
            if (entry != null) {
                var customExecuteActivity = new CustomExecuteActivity(entry.Execute, entry.BookmarkCallback) {
                    Version = entry.Version,
                    Bookmark = entry.Bookmark,
                    Name = entry.Name,
                };
                activity.NextActivities.Add(customExecuteActivity);
                activity = customExecuteActivity;
            }

            var exit = state.Exit;
            if (exit != null) {
                var customExecuteActivity = new CustomExecuteActivity(exit.Execute, exit.BookmarkCallback) {
                    Version = exit.Version,
                    Bookmark = exit.Bookmark,
                    Name = exit.Name
                };
                activity.NextActivities.Add(customExecuteActivity);
                activity = customExecuteActivity;
            }
            
            //transition
            foreach (var transition in state.Transitions) {
                var transitionTrigger = transition.Trigger;
                var customExecuteActivity = new CustomExecuteActivity(transitionTrigger.Execute,transitionTrigger.BookmarkCallback) {
                    Version = transitionTrigger.Version,
                    Bookmark = transitionTrigger.Bookmark,
                    Name = transitionTrigger.Name,
                };
                activity.NextActivities.Add(customExecuteActivity);
                foreach (var path in transition.TransitionPaths) {
                    
                }
            }
        }

        private void InternalRun(IExecuteActivity activity, PipelineContext context)
        {
            var execute = activity.Execute(context);
            activity.Executed = true;
            if (!execute) {
                return;
            }

            if (activity.IsHangUped) {
                return;
            }

            if (activity.NextActivities.Count == 0) {
                return;
            }

            foreach (var nextActivity in activity.NextActivities) {
                InternalRun(nextActivity, context);
            }
        }
    }
}