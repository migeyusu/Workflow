using System;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Persistent
{
    /*
     * 暂时不用
     */
    public class StateModel:IActivity
    {
        public Guid Version { get; set; }

        public string Name { get; set; }

        public RunningActivityModel Entry { get; set; }

        public RunningActivityModel Exit { get; set; }
    }
}