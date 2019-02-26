using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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

        internal bool IsWaiting { get; set; }

        internal string WaitingBookmark { get; set; }

        /// <summary>
        /// 用于内部activity
        /// </summary>
        internal ConcurrentDictionary<string, object> InternalPipe { get; set; } =
            new ConcurrentDictionary<string, object>();

        internal ConcurrentDictionary<string, string> PersistableLocals { get; set; } =
            new ConcurrentDictionary<string, string>();

        internal Dictionary<string, IExecuteActivity> SuspendedActivities { get; set; } =
            new Dictionary<string, IExecuteActivity>();

        public void Set(string name, string value)
        {
            PersistableLocals.AddOrUpdate(name, s => value, (s, s1) => value);
        }

        public string Get(string name)
        {
            return PersistableLocals.TryGetValue(name, out var value) ? value : string.Empty;
        }

        public void Remove(string key)
        {
            PersistableLocals.TryRemove(key, out var value);
        }

        internal void InternalRequestHangUp(IExecuteActivity activity)
        {
            if (SuspendedActivities.ContainsKey(WaitingBookmark)) {
                throw new DuplicateNameException("The same bookmark name!");
            }
            SuspendedActivities.Add(WaitingBookmark, activity);
            IsWaiting = false;
            WaitingBookmark = string.Empty;
        }

        /// <summary>
        /// 挂起当前activity,只允许单线程调用
        /// </summary>
        /// <param name="bookmark"></param>
        public void WaitOn(string bookmark)
        {
            IsWaiting = true;
            WaitingBookmark = bookmark;
        }
    }
}