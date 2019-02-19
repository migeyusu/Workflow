using System;
using WorkflowFacilities.Consumer;

namespace WorkflowFacilities.Persistent
{
    public class Field:IDisposable
    {
        

        public  T Get<T>() where T:StateMachine
        {
            
            var stateMachine = new StateMachine();
            
        }

        public void Save()
        {

        }
        
        public void Dispose()
        { }
    }
}