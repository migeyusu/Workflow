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
        public event Action OnIdle;

        public event Action OnCompleted;

        private readonly StateMachine _stateMachine;

        private readonly PipelineContext _context;

        public StateMachineScheduler(StateMachine stateMachine)
        {
            this._stateMachine = stateMachine;
            _context = stateMachine.Context;
        }

        public virtual void RiseIdle()
        {
            OnIdle?.Invoke();
        }

        public virtual void RiseCompleted()
        {
            OnCompleted?.Invoke();
        }

        /*
         * 翻译成可执行的activity
         */
        public static StartActiviy Translate(StateMachineTemplate stateMachine)
        {
            var activitiesMapping = new Dictionary<State, IExecuteActivity>();
            var startActivity = new StartActiviy();
            stateMachine.StartState.InternalTranslate(startActivity, activitiesMapping);
            return startActivity;
        }

        /// <summary>
        /// 运行初始化后的
        /// </summary>
        /// <param name="dictionary"></param>
        public void Run(IDictionary<string, string> dictionary = null)
        {
            if (_stateMachine.IsCompleted) {
                throw new ArgumentException("不能运行已经结束的statemachine！");
            }

            //only for first run
            if (_context.IsRunning) {
                return;
            }

            if (dictionary != null) {
                foreach (var pair in dictionary) {
                    _context.LocalVariableDictionary.TryAdd(pair.Key, pair.Value);
                }
            }

            InternalRun(_stateMachine.ExecuteActivityChainEntry, _stateMachine.Context);
            _context.IsRunning = true;
            if (_context.IsCompleted) {
                _context.IsRunning = false;
                this.RiseCompleted();
            }

            if (!_context.IsCompleted) {
                this.RiseIdle();
            }
        }

        private void InternalRun(IExecuteActivity activity, PipelineContext context)
        {
            var execute = activity.Execute(context);
            if (!execute) {
                return;
            }

            if (context.IsWaiting) {
                context.InternalRequestHangUp(activity);
                context.IsWaiting = false;
                return;
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

        public void ResumeBookmark(string bookmark, object value)
        {
            if (string.IsNullOrEmpty(bookmark)) {
                throw new ArgumentNullException("bookmark不允许为空");
            }

            if (_context.SuspendedActivities.TryGetValue(bookmark, out IExecuteActivity executeActivity)) {
                executeActivity.BookmarkCallback(_context, bookmark, value);
                _context.SuspendedActivities.Remove(bookmark);
                foreach (var nextActivity in executeActivity.NextActivities) {
                    InternalRun(nextActivity, _context);
                }
            }
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
                            DisplayName = activityModel.DisplayName,
                        };
                    break;
                case RunningActivityType.Set:
                    executeActivity = new StateSetExecuteActivity() {
                        DisplayName = activityModel.DisplayName,
                    };
                    break;
                case RunningActivityType.Start:
                    executeActivity = new StartActiviy();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            executeActivity.Id = activityModel.Id;
            cache.Add(activityModel.Id, executeActivity);
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
                DisplayName = executeActivity.DisplayName,
                Id = executeActivity.Id,
                ActivityType = executeActivity.ActivityType,
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