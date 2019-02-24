using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowFacilities.Persistent
{
    public class StateMachineModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 不直接引用template，因为template只会在内存中
        /// </summary>
        public virtual StateMachineTemplateModel TemplateModel { get; set; }

        #region context

        public string CurrentStateName { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsRunning { get; set; }

        public byte[] LocalVariousDictionary { get; set; }

        public virtual List<RunningActivityModel> SuspendedActivityModels { get; set; }

        #endregion

        public StateMachineModel()
        {
            //Id = Guid.NewGuid();
            SuspendedActivityModels = new List<RunningActivityModel>();
        }
    }
}