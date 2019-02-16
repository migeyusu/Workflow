﻿using System;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace WorkflowFacilities.Persistent
{
    public class StateModel:IActivity
    {
        public Guid Version { get; set; }

        public string Name { get; set; }

        public ActivityModel Entry { get; set; }

        public ActivityModel Exit { get; set; }

        public Transition Type { get; set; }

    }
}