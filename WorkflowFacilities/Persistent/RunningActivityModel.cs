using System;
using System.Collections.Generic;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Persistent
{
    /// <summary>
    /// 持久化属性包括可以同过创建新实例指定
    /// </summary>
    public class RunningActivityModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// clarify activity type,used to mapping:if guid exist in 
        /// </summary>
        public Guid Version { get; set; }

        public string Name { get; set; }

        public string Bookmark { get; set; }

        public List<RunningActivityModel> RunningActivityModels { get; set; }

        public RunningActivityModel()
        {
            Id = Guid.NewGuid();
            RunningActivityModels=new List<RunningActivityModel>();
        }
    }
}