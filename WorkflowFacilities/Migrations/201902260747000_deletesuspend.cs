namespace WorkflowFacilities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletesuspend : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StateMachineModelRunningActivityModels", "StateMachineModel_Id", "dbo.StateMachineModels");
            DropForeignKey("dbo.StateMachineModelRunningActivityModels", "RunningActivityModel_Id", "dbo.RunningActivityModels");
            DropIndex("dbo.StateMachineModelRunningActivityModels", new[] { "StateMachineModel_Id" });
            DropIndex("dbo.StateMachineModelRunningActivityModels", new[] { "RunningActivityModel_Id" });
            DropTable("dbo.StateMachineModelRunningActivityModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StateMachineModelRunningActivityModels",
                c => new
                    {
                        StateMachineModel_Id = c.Guid(nullable: false),
                        RunningActivityModel_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.StateMachineModel_Id, t.RunningActivityModel_Id });
            
            CreateIndex("dbo.StateMachineModelRunningActivityModels", "RunningActivityModel_Id");
            CreateIndex("dbo.StateMachineModelRunningActivityModels", "StateMachineModel_Id");
            AddForeignKey("dbo.StateMachineModelRunningActivityModels", "RunningActivityModel_Id", "dbo.RunningActivityModels", "Id", cascadeDelete: true);
            AddForeignKey("dbo.StateMachineModelRunningActivityModels", "StateMachineModel_Id", "dbo.StateMachineModels", "Id", cascadeDelete: true);
        }
    }
}
