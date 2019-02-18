using System.Collections.Generic;
using WorkflowFacilities.Consumer;

namespace WorkflowFacilities.Running
{
    public class StateMachineScheduler
    {
        private Dictionary<State, IExecuteActivity> _activitiesMapping;

        /*
         * 翻译成可执行的activity
         */
        
        public StartActiviy Translate(StateMachine stateMachine)
        {
            _activitiesMapping = new Dictionary<State, IExecuteActivity>();
            var startActiviy = new StartActiviy();
            InternalTranslate(stateMachine.StartState, startActiviy);
            stateMachine.CurrentActivity = startActiviy;
            return startActiviy;
        }

        private void InternalTranslate(State state, IExecuteActivity executeActivity)
        {
            var activity = executeActivity;
            var stateEmptyExecuteActivity = new StateEmptyExecuteActivity {
                ParentActivity = state,
                Name = state.Name
            };
            _activitiesMapping.Add(state, stateEmptyExecuteActivity);
            activity.NextActivities.Add(stateEmptyExecuteActivity);
            activity = stateEmptyExecuteActivity;
            var entry = state.Entry;
            if (entry != null) {
                var customExecuteActivity = new CustomExecuteActivity(entry.Execute,
                    entry.BookmarkCallback) {
                    Version = entry.Version,
                    Bookmark = entry.Bookmark,
                    Name = entry.Name,
                    ParentActivity = state
                };
                activity.NextActivities.Add(customExecuteActivity);
                activity = customExecuteActivity;
            }

            var exit = state.Exit;
            if (exit != null) {
                var customExecuteActivity = new CustomExecuteActivity(exit.Execute,
                    exit.BookmarkCallback) {
                    Version = exit.Version,
                    Bookmark = exit.Bookmark,
                    Name = exit.Name,
                    ParentActivity = state
                };
                activity.NextActivities.Add(customExecuteActivity);
                activity = customExecuteActivity;
            }

            foreach (var transition in state.Transitions) {
                var endActivity = activity;
                var trigger = transition.Trigger;
                if (trigger != null) {
                    var customExecuteActivity = new CustomExecuteActivity(trigger.Execute,
                        trigger.BookmarkCallback) {
                        Version = trigger.Version,
                        Bookmark = trigger.Bookmark,
                        Name = trigger.Name,
                    };
                    endActivity.NextActivities.Add(customExecuteActivity);
                    endActivity = customExecuteActivity;
                }

                foreach (var path in transition.TransitionPaths) {
                    var conditionActivity = new ConditionActivity(path.ConditionFunc);
                    endActivity.NextActivities.Add(conditionActivity);
                    IExecuteActivity pathactivity = conditionActivity;
                    var aciton = path.Aciton;
                    if (aciton != null) {
                        var customExecuteActivity = new CustomExecuteActivity(aciton.Execute,
                            aciton.BookmarkCallback) {
                            Version = aciton.Version,
                            Bookmark = aciton.Bookmark,
                            Name = aciton.Name,
                        };
                        pathactivity.NextActivities.Add(customExecuteActivity);
                        pathactivity = customExecuteActivity;
                    }

                    var pathTo = path.To;
                    if (_activitiesMapping.TryGetValue(pathTo, out IExecuteActivity nextExecuteActivity)) {
                        pathactivity.NextActivities.Add(nextExecuteActivity);
                    }
                    else {
                        InternalTranslate(pathTo, pathactivity);
                    }
                }
            }
        }

        
        
        /// <summary>
        /// 运行初始化后的
        /// </summary>
        /// <param name="stateMachine"></param>
        public void Run(StateMachine stateMachine)
        {
            if (stateMachine.CurrentActivity==null) {
                Translate(stateMachine);
            }
            stateMachine.
            InternalRun(stateMachine.CurrentActivity, stateMachine.Context);
        }
        
        

        private void InternalRun(IExecuteActivity activity, PipelineContext context)
        {
            var execute = activity.Execute(context);
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