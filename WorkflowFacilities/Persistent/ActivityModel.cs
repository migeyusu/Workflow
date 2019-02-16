using System;
using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Persistent
{
    /// <summary>
    /// 用于持久化
    /// </summary>
    public class ActivityModel:IActivity,IExecutable
    {
        public Guid Version { get; set; }

        public string Name { get; set; }

        public bool IsHangUped { get; set; }

        public bool Executed { get; set; }

    }
}