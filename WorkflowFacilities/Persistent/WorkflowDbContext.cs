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

        public DbSet<StateMachineModel> StateMachines { get; set; }
        
        public DbSet<RunningActivityModel> ActivityModels { get; set; }

        public DbSet<StateMachineTemplateModel> StateMachineTemplateModels { get; set; }
    }
}