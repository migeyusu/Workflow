using System;
using WorkflowFacilities.Consumer;
using WorkflowFacilities.Running;

namespace WorkflowFacilities
{
    public class Field:IDisposable
    {
        
        public  T Get<T>() where T:StateMachine
        {

        }

        public void Dispose()
        { }
    }
}