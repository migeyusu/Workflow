using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Persistent;
using WorkflowFacilities.Running;

namespace WorkflowFacilities
{
    public class WorkflowFact
    {
        private static readonly List<StateMachineTemplate> _templates = new List<StateMachineTemplate>();

        public static List<StateMachineTemplate> Templates {
            get { return _templates; }
        }

        public static void Register(StateMachineTemplate template)
        {
            if (_templates.FirstOrDefault(
                    machineTemplate => machineTemplate.Version == template.Version) != null) {
                return;
            }
            _templates.Add(template);
        }

        internal static StateMachineTemplate Get(Guid versionGuid) =>
            _templates.FirstOrDefault((template => template.Version == versionGuid));

        /// <summary>
        /// 访问的入口
        /// </summary>
        /// <param name="constring">ef配置字段：
        ///  <add  name="constring"  connectionString="xx"  providerName="System.Data.SqlClient" />
        /// </param>
        /// <returns></returns>
        public static Field OpenField(string constring)
        {
            var workflowDbContext = new WorkflowDbContext(constring);
            return new Field(workflowDbContext);
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
                        new CustomExecuteActivity(customActivity.Execute, customActivity.BookmarkCallback) {
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
                Bookmark = executeActivity.Bookmark
            };
            cache.Add(activityModel.Id, activityModel);
            foreach (var nextActivity in executeActivity.NextActivities) {
                if (cache.TryGetValue(nextActivity.Id)) { }

                var runningActivityModel = Serialize(nextActivity, cache);
                activityModel.RunningActivityModels.Add(runningActivityModel);
            }

            return activityModel;
        }
    }
}