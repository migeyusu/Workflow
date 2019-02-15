using System.Data.Entity;

namespace WorkflowFacilities
{
    public class WorkflowDbContext:DbContext
    {
        public DbSet<StateMachineModel> StateMachines { get; set; }

        public DbSet<StateModel> States { get; set; }

        public DbSet<TransitionModel> Transitions { get; set; }

        public DbSet<ActivityModel> ActivityModels { get; set; }
    }
}