using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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

        public bool IsCompleted { get; internal set; }

        public bool IsRunning { get; internal set; }

        public bool IsWaiting { get; set; }

        public KeyValuePair<string,object> ResumingBookmark { get; internal set; }

        internal ConcurrentDictionary<string, string> LocalVariableDictionary { get; set; } =
            new ConcurrentDictionary<string, string>();

        internal Dictionary<string, IExecuteActivity> SuspendedActivities { get; set; } =
            new Dictionary<string, IExecuteActivity>();

        public void Set(string name, string value)
        {
            LocalVariableDictionary.AddOrUpdate(name, s => value, (s, s1) => value);
        }

        public string Get(string name)
        {
            return LocalVariableDictionary.TryGetValue(name, out var value) ? value : string.Empty;
        }

        internal void InternalRequestHangUp(IExecuteActivity activity)
        {
            activity.IsHangUped = true;
            SuspendedActivities.Add(activity.Bookmark, activity);
        }

        /// <summary>
        /// 挂起当前activity,只允许单线程调用
        /// </summary>
        /// <param name="customActivity"></param>
        public void WaitOn()
        {
            IsWaiting = true;
        }
    }
}