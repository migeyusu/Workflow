using System;
using WorkflowFacilities.Consumer;

namespace WorkflowFacilities.Persistent
{
    public class Field:IDisposable
    {
        

        public StateMachine Get()
        {
            var stateMachine = new StateMachine();


            return stateMachine;
        }

        public void Save()
        {

        }
        
        public void Dispose()
        { }
    }
}