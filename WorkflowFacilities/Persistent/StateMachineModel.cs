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


        //public virtual List<RunningActivityModel> SuspendedActivityModels { get; set; }
        /*由于需要允许运行时的activity可变，所以仍然采用一次生成的固定的运行链，但是运行链包含有可以控制runningactivity链的activity*/
        /// <summary>
        /// 脱离固定的运行链，可以增加灵活性
        /// </summary>
        public virtual List<SuspendedRunningActivityModel> SuspendedRunningActivityModels { get; set; }

        #endregion

        public StateMachineModel()
        {
            //Id = Guid.NewGuid();
//            SuspendedActivityModels = new List<RunningActivityModel>();
            SuspendedRunningActivityModels = new List<SuspendedRunningActivityModel>();
        }
    }
}