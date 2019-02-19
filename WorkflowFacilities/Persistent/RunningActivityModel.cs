using System;
using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Persistent
{
    /// <summary>
    /// 持久化属性包括可以同过创建新实例指定
    /// </summary>
    public class RunningActivityModel : IActivity
    {
        public Guid Id { get; set; }

        public Guid Version { get; set; }

        public string Name { get; set; }

        public string Bookmark { get; set; }

        public IList<RunningActivityModel> RunningActivityModels { get; set; }
    }
}