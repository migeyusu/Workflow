using System.Collections.Concurrent;
using System.Collections.Generic;
using WorkflowFacilities.Consumer;

namespace WorkflowFacilities.Running
{
    /// <summary>
    /// 负责在整个workflow里传递变量、交互
    /// 允许被挂起
    /// 运行结束后赋值给statemachine
    /// </summary>
    public class PipelineContext
    {
        public string CurrentStateName { get; internal set; }

        public bool IsCompleted { get; set; }

        public KeyValuePair<string,object> ResumingBookmark { get; set; }

        public ConcurrentDictionary<string, string> LocalVariableDictionary { get; set; } =
            new ConcurrentDictionary<string, string>();

        public Dictionary<string, IExecuteActivity> WaitingForBookmarkList { get; set; } =
            new Dictionary<string, IExecuteActivity>();

        public void Set(string name, string value)
        {
            LocalVariableDictionary.AddOrUpdate(name, s => value, (s, s1) => value);
        }

        public string Get(string name)
        {
            return LocalVariableDictionary.TryGetValue(name, out var value) ? value : string.Empty;
        }

        private void InternalRequestHangUp(IExecuteActivity activity)
        {
            activity.IsHangUped = true;
            WaitingForBookmarkList.Add(activity.Bookmark, activity);
        }

        /// <summary>
        /// 挂起当前activity
        /// </summary>
        /// <param name="customActivity"></param>
        public void WaitOn(ICustomActivity customActivity)
        {
            var executeActivity = customActivity as IExecuteActivity;
            InternalRequestHangUp(executeActivity);
        }
    }
}