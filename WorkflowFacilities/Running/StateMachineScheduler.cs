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
            var activitiesMapping = new Dictionary<Guid, IExecuteActivity>();
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
                    _context.PersistableLocals.TryAdd(pair.Key, pair.Value);
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

            if (_context.SuspendedActivities.TryGetValue(bookmark, out var executeActivity)) {
                executeActivity.BookmarkCallback(_context, bookmark, value);
                _context.SuspendedActivities.Remove(bookmark);
                foreach (var nextActivity in executeActivity.NextActivities) {
                    InternalRun(nextActivity, _context);
                }
            }
        }

        /// <summary>
        /// generate running chain, some of them are different from translated
        /// </summary>
        /// <param name="activityModel"></param>
        /// <param name="cache"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        internal static IExecuteActivity Deserialize(RunningActivityModel activityModel,
            IDictionary<Guid, IExecuteActivity> cache, StateMachineTemplate template)
        {
            IExecuteActivity executeActivity;
            var activityModelVersion = activityModel.Version;
            switch (activityModel.ActivityType) {
                case RunningActivityType.Condition:
                    var transitionPath =
                        template.CustomActivities.FirstOrDefault(path => path.Version == activityModelVersion);
                    if (transitionPath == null) {
                        throw new ObjectNotFoundException(
                            $"无法从模板{template.Name}找到version为{activityModelVersion}的transitionpath！");
                    }

                    executeActivity = new ConditionActivity(transitionPath);
                    break;
                case RunningActivityType.Custom:
                    var customActivity = template.CustomActivities.FirstOrDefault(
                        activity => activity.Version == activityModelVersion);
                    if (customActivity == null) {
                        throw new ObjectNotFoundException(
                            $"无法从模板{template.Name}找到version为{activityModelVersion}的activity！");
                    }

                    executeActivity = new CustomExecuteActivity(customActivity);
                    break;
                case RunningActivityType.Set:
                    executeActivity = new StateSetExecuteActivity() {
                        DisplayName = activityModel.DisplayName,
                    };
                    break;
                case RunningActivityType.Start:
                    executeActivity = new StartActiviy();
                    break;
                case RunningActivityType.ParallelStart:
                    executeActivity = new ParallelStartActivity() {Version = activityModelVersion,};
                    break;
                case RunningActivityType.ParallelEnd:
                    executeActivity = new ParallelEndActivity() {Version = activityModelVersion};
                    break;
                case RunningActivityType.ParallelForeachEnty:
                    var parallelForeach = template.CustomActivities
                            .FirstOrDefault(activity => activity.Version == activityModelVersion)
                        as ParallelForeach;
                    executeActivity = new ParallelForeachEntry(parallelForeach);
                    break;
                case RunningActivityType.ParallelForeachLoopEnd:
                    executeActivity = new ParallelForeachLoopEnd();
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