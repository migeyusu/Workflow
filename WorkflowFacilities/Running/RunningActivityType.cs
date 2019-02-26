namespace WorkflowFacilities.Running
{
    public enum RunningActivityType : int
    {
        Condition = 1,

        Custom = 2,

        Set = 4,

        Start = 8,

        ParallelStart = 16,

        ParallelEnd = 32,
        
    }
}