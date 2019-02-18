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

        private ConcurrentDictionary<string, string> _localVariableDictionary =
            new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string, string> LocalVariableDictionary {
            get { return _localVariableDictionary; }
            set { _localVariableDictionary = value; }
        }

        public Dictionary<string,IExecuteActivity> WaitingForBookmarkList {
            get { return _waitingForBookmarkList; }
            set { _waitingForBookmarkList = value; }
        }
        
        private Dictionary<string,IExecuteActivity> _waitingForBookmarkList = new Dictionary<string, IExecuteActivity>();

        public void Set(string name, string value)
        {
            _localVariableDictionary.AddOrUpdate(name, s => value, (s, s1) => value);
        }

        public string Get(string name)
        {
            return _localVariableDictionary.TryGetValue(name, out var value) ? value : string.Empty;
        }

        private void InternalRequestHangUp(IExecuteActivity activity)
        {
            activity.IsHangUped = true;
            _waitingForBookmarkList.Add(activity.Bookmark,activity);
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