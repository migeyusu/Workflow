using System.Collections.Generic;

namespace WorkflowFacilities.Running
{
    public interface IExecutable
    {
        /// <summary>
        /// 表示是否被挂起
        /// </summary>
        bool IsHangUped { get; set; }

        bool Executed { get; set; }
    }
}