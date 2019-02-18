using System;
using System.Collections.Generic;

namespace WorkflowFacilities.Persistent
{
    public class StateMachineModel
    {
        public Guid Id { get; set; }

        public RunningActivityModel StartActivityModel { get; set; }

        public List<RunningActivityModel> WaitingRunningActivityModels { get; set; }

        public string CurrentStateName { get; set; }

        public string LocalVariousDictionary { get; set; }
        
    }
}