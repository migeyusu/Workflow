﻿using System;

namespace WorkflowFacilities
{
    public class TransitionPath
    {
        public ICustomActivity To { get; set; }

        public ICustomActivity Aciton { get; set; }

        public Func<bool> ConditionFunc;
    }
}