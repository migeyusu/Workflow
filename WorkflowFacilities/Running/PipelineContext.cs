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
        private ConcurrentDictionary<string, string> _localVariableDictionary =
            new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string, string> LocalVariableDictionary {
            get { return _localVariableDictionary; }
            set { _localVariableDictionary = value; }
        }

        public List<string> WaitingForBookmarkList {
            get { return _waitingForBookmarkList; }
            set { _waitingForBookmarkList = value; }
        }

        private List<string> _waitingForBookmarkList = new List<string>();


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
            _waitingForBookmarkList.Add(activity.Bookmark);
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