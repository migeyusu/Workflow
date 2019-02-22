using System.Data.Entity;

namespace WorkflowFacilities.Persistent
{
    public class WorkflowDbContext:DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="constring">ef配置字段</param>
        public WorkflowDbContext(string constring):base(constring)
        {
            
        }

        public WorkflowDbContext():base("WorkflowDb")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StateMachineModel>()
                .HasMany((s) => s.SuspendedActivityModels)
                .WithMany();
//                .WithRequired()
//                .HasForeignKey(model => model.Id);
            modelBuilder.Entity<StateMachineTemplateModel>()
                .HasMany((s) => s.RunningActivityModels)
                .WithMany();
//                .WithRequired()
//                .HasForeignKey((model => model.Id));
            modelBuilder.Entity<RunningActivityModel>()
                .HasMany((s) => s.RunningActivityModels)
                .WithMany();
            base.OnModelCreating(modelBuilder);
            
        }


        public DbSet<StateMachineModel> StateMachineModels { get; set; }
        
        public DbSet<RunningActivityModel> ActivityModels { get; set; }

        public DbSet<StateMachineTemplateModel> StateMachineTemplateModels { get; set; }
    }
}