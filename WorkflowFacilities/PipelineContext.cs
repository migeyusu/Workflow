using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace WorkflowFacilities
{
    /// <summary>
    /// 允许在整个workflow里传递变量
    /// 允许被挂起
    /// </summary>
    public class PipelineContext
    {
        private ConcurrentDictionary<string, string> _localVariableDictionary =
            new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string, string> LocalVariableDictionary {
            get { return _localVariableDictionary; }
            set { _localVariableDictionary = value; }
        }

        
        public void Set(string name, string value)
        {
            _localVariableDictionary.AddOrUpdate(name, s => value, (s, s1) => value);
        }

        public string Get(string name)
        {
            return _localVariableDictionary.TryGetValue(name, out var value) ? value : string.Empty;
        }

        public void RequestHangUp(ICustomActivity activity)
        {
            
        }
        
    }
}