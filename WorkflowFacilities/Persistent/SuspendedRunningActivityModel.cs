using System;

namespace WorkflowFacilities.Persistent
{
    public class SuspendedRunningActivityModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 动态存储bookmark
        /// </summary>
        public string BookmarkName { get; set; }

        public RunningActivityModel RunningActivityModel { get; set; }

        public SuspendedRunningActivityModel()
        {
            Id=Guid.NewGuid();
        }
    }
}