using WorkflowFacilities.Consumer;

namespace WorkflowFacilities
{
    public interface IWorkflowTemplateRegister
    {
        IWorkflowTemplateRegister Register<T>() where T : StateMachineTemplate;
    }
}