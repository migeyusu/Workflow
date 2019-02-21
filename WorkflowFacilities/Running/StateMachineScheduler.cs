using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Persistent;

namespace WorkflowFacilities.Running
{
    public class StateMachineScheduler
    {
        /*
         * 翻译成可执行的activity
         */

        public static StartActiviy Translate(StateMachineTemplate stateMachine)
        {
            var activitiesMapping = new Dictionary<State, IExecuteActivity>();
            var startActivity = new StartActiviy();
            InternalTranslate(stateMachine.StartState, startActivity, activitiesMapping);
            return startActivity;
        }

        private static void InternalTranslate(State state, IExecuteActivity executeActivity,
            IDictionary<State, IExecuteActivity> mapping)
        {
            var activity = executeActivity;
            var stateEmptyExecuteActivity = new StateSetExecuteActivity {
                Name = state.Name
            };
            mapping.Add(state, stateEmptyExecuteActivity);
            activity.NextActivities.Add(stateEmptyExecuteActivity);
            activity = stateEmptyExecuteActivity;
            var entry = state.Entry;
            if (entry != null) {
                var customExecuteActivity = new CustomExecuteActivity(entry) {
                    Version = entry.Version,
                    Bookmark = entry.Bookmark,
                    Name = entry.Name
                };
                activity.NextActivities.Add(customExecuteActivity);
                activity = customExecuteActivity;
            }

            var exit = state.Exit;
            if (exit != null) {
                var customExecuteActivity = new CustomExecuteActivity(exit);
                activity.NextActivities.Add(customExecuteActivity);
                activity = customExecuteActivity;
            }

            foreach (var transition in state.Transitions) {
                var endActivity = activity;
                var trigger = transition.Trigger;
                if (trigger != null) {
                    var customExecuteActivity = new CustomExecuteActivity(trigger);
                    endActivity.NextActivities.Add(customExecuteActivity);
                    endActivity = customExecuteActivity;
                }

                foreach (var path in transition.TransitionPaths) {
                    var pathConditionFunc = path.ConditionFunc;
                    var pathactivity = endActivity;
                    if (pathConditionFunc != null) {
                        var conditionActivity = new ConditionActivity(pathConditionFunc);
                        pathactivity.NextActivities.Add(conditionActivity);
                        pathactivity = conditionActivity;
                    }

                    var aciton = path.Aciton;
                    if (aciton != null) {
                        var customExecuteActivity = new CustomExecuteActivity(aciton);
                        pathactivity.NextActivities.Add(customExecuteActivity);
                        pathactivity = customExecuteActivity;
                    }

                    var pathTo = path.To;
                    if (mapping.TryGetValue(pathTo, out IExecuteActivity nextExecuteActivity)) {
                        pathactivity.NextActivities.Add(nextExecuteActivity);
                    }
                    else {
                        InternalTranslate(pathTo, pathactivity, mapping);
                    }
                }
            }
        }


        /// <summary>
        /// 运行初始化后的
        /// </summary>
        /// <param name="stateMachine"></param>
        /// <param name="param">运行前输入参数</param>
        public void Run(StateMachine stateMachine, IDictionary<string, string> param = null)
        {
            //first run
            if (!stateMachine.IsRunning) {
                if (param != null) {
                    foreach (var pair in param) {
                        var stateMachineContext = stateMachine.Context;
                        stateMachineContext.Set(pair.Key, pair.Value);
                    }
                }

                InternalRun(stateMachine.ExecuteActivityChainEntry, stateMachine.Context);
            }
            else {
                foreach (var executeActivity in stateMachine.Context.SuspendedActivities.Values) {
                    InternalRun(executeActivity, stateMachine.Context);
                }
            }

            stateMachine.Context.IsRunning = true;
        }


        private void InternalRun(IExecuteActivity activity, PipelineContext context)
        {
            if (activity.IsHangUped && activity.Bookmark != null &&
                context.ResumingBookmark.Key == activity.Bookmark) {
                activity.BookmarkCallback(context);
                context.ResumingBookmark = new KeyValuePair<string, object>();
                activity.IsHangUped = false;
            }
            else {
                var execute = activity.Execute(context);
                if (!execute) {
                    return;
                }

                if (context.IsWaiting) {
                    context.InternalRequestHangUp(activity);
                    return;
                }
            }

            //end
            if (activity.NextActivities.Count == 0) {
                context.IsCompleted = true;
                return;
            }

            foreach (var nextActivity in activity.NextActivities) {
                InternalRun(nextActivity, context);
            }
        }

        public void ResumeBookMark(StateMachine stateMachine, string name, string value)
        {
            var stateMachineContext = stateMachine.Context;
            stateMachineContext.ResumingBookmark = new KeyValuePair<string, object>(name, value);
            Run(stateMachine);
        }

        internal static IExecuteActivity Deserialize(RunningActivityModel activityModel,
            IDictionary<Guid, IExecuteActivity> cache, StateMachineTemplate template)
        {
            IExecuteActivity executeActivity;
            switch (activityModel.ActivityType) {
                case RunningActivityType.Condition:
                    var transitionPath =
                        template.TransitionPaths.FirstOrDefault(path => path.Version == activityModel.Version);
                    if (transitionPath == null) {
                        throw new ObjectNotFoundException(
                            $"无法从模板{template.Name}找到version为{activityModel.Version}的transitionpath！");
                    }

                    executeActivity = new ConditionActivity(transitionPath.ConditionFunc);
                    break;
                case RunningActivityType.Custom:
                    var customActivity = template.CustomActivities.FirstOrDefault(
                        activity => activity.Version == activityModel.Version);
                    if (customActivity == null) {
                        throw new ObjectNotFoundException(
                            $"无法从模板{template.Name}找到version为{activityModel.Version}的activity！");
                    }

                    executeActivity =
                        new CustomExecuteActivity(customActivity) {
                            Version = activityModel.Version,
                            Bookmark = activityModel.Bookmark,
                            Name = activityModel.Name,
                        };
                    break;
                case RunningActivityType.Set:
                    executeActivity = new StateSetExecuteActivity() {
                        Name = activityModel.Name,
                    };
                    break;
                case RunningActivityType.Start:
                    executeActivity = new StartActiviy();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            executeActivity.Id = activityModel.Id;
            cache.Add(activityModel.Id,executeActivity);
            foreach (var nextActivity in activityModel.RunningActivityModels) {
                if (!cache.TryGetValue(nextActivity.Id, out var activity)) {
                    activity = Deserialize(nextActivity, cache, template);
                }

                executeActivity.NextActivities.Add(activity);
            }

            return executeActivity;
        }


        internal static RunningActivityModel Serialize(IExecuteActivity executeActivity,
            IDictionary<Guid, RunningActivityModel> cache)
        {
            var activityModel = new RunningActivityModel {
                Version = executeActivity.Version,
                Name = executeActivity.Name,
                Bookmark = executeActivity.Bookmark,
                Id = executeActivity.Id
            };
            cache.Add(executeActivity.Id, activityModel);
            foreach (var nextActivity in executeActivity.NextActivities) {
                if (!cache.TryGetValue(nextActivity.Id, out var runningActivityModel)) {
                    runningActivityModel = Serialize(nextActivity, cache);
                }

                activityModel.RunningActivityModels.Add(runningActivityModel);
            }

            return activityModel;
        }
    }
}