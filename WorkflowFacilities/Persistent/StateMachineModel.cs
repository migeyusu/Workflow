using System;
using System.Collections.Generic;

namespace WorkflowFacilities.Persistent
{
    public class StateMachineModel
    {
        public Guid Id { get; set; }

        public Guid StateMachineTemplateVersion { get; set; }

        public RunningActivityModel StartActivityModel { get; set; }
        
        public string CurrentStateName { get; set; }

        public bool IsCompleted { get; set; }
        
        public string LocalVariousDictionary { get; set; }

        public List<RunningActivityModel> WaitingRunningActivityModels { get; set; }

        public StateMachineModel()
        {
            //Id = Guid.NewGuid();
            WaitingRunningActivityModels=new List<RunningActivityModel>();
        } 

    }
}