using System;

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