using System.Data.Entity;

namespace WorkflowFacilities.Persistent
{
    public class WorkflowDbContext:DbContext
    {
        public DbSet<StateMachineModel> StateMachines { get; set; }
        
        

        public DbSet<RunningActivityModel> ActivityModels { get; set; }
    }
}