using System;

namespace WorkflowFacilities
{
    /// <summary>
    /// 用于持久化
    /// </summary>
    public class ActivityModel:IActivity
    {
        public Guid Version { get; set; }
    }
}