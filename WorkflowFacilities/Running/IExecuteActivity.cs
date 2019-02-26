using System;
using System.Collections.Generic;
using WorkflowFacilities.Consumer;

namespace WorkflowFacilities.Running
{
    /// <summary>
    /// 可执行activity的最小单元
    /// </summary>
    public interface IExecuteActivity : ICustomActivity
    {
        Guid Id { get; set; }
        RunningActivityType ActivityType { get; set; }
//        string Bookmark { get; set; }
        IList<IExecuteActivity> NextActivities { get; set; }
    }
}