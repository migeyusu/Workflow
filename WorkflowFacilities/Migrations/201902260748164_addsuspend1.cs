namespace WorkflowFacilities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsuspend1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SuspendedRunningActivityModels", "StateMachineModel_Id", c => c.Guid());
            CreateIndex("dbo.SuspendedRunningActivityModels", "StateMachineModel_Id");
            AddForeignKey("dbo.SuspendedRunningActivityModels", "StateMachineModel_Id", "dbo.StateMachineModels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SuspendedRunningActivityModels", "StateMachineModel_Id", "dbo.StateMachineModels");
            DropIndex("dbo.SuspendedRunningActivityModels", new[] { "StateMachineModel_Id" });
            DropColumn("dbo.SuspendedRunningActivityModels", "StateMachineModel_Id");
        }
    }
}
