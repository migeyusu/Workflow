using System;
using System.Collections.Generic;

namespace WorkflowFacilities.Persistent
{
    public class StateMachineModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 不直接引用template，因为template会长期储存在内存缓存中
        /// </summary>
        public Guid StateMachineTemplateVersion { get; set; }

        public string CurrentStateName { get; set; }

        public bool IsCompleted { get; set; }

        public string LocalVariousDictionary { get; set; }

        public List<RunningActivityModel> SuspendedActivityModels { get; set; }

        public StateMachineModel()
        {
            //Id = Guid.NewGuid();
            SuspendedActivityModels = new List<RunningActivityModel>();
        }
    }
}