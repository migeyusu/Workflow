namespace WorkflowFacilities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RunningActivityModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ActivityType = c.Int(nullable: false),
                        Version = c.Guid(nullable: false),
                        Name = c.String(),
                        Bookmark = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StateMachineModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CurrentStateName = c.String(),
                        IsCompleted = c.Boolean(nullable: false),
                        IsRunning = c.Boolean(nullable: false),
                        LocalVariousDictionary = c.String(),
                        TemplateModel_Version = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StateMachineTemplateModels", t => t.TemplateModel_Version)
                .Index(t => t.TemplateModel_Version);
            
            CreateTable(
                "dbo.StateMachineTemplateModels",
                c => new
                    {
                        Version = c.Guid(nullable: false),
                        TemplateClassTypeName = c.String(),
                        StartActivityModel_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Version)
                .ForeignKey("dbo.RunningActivityModels", t => t.StartActivityModel_Id)
                .Index(t => t.StartActivityModel_Id);
            
            CreateTable(
                "dbo.RunningActivityModelRunningActivityModels",
                c => new
                    {
                        RunningActivityModel_Id = c.Guid(nullable: false),
                        RunningActivityModel_Id1 = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.RunningActivityModel_Id, t.RunningActivityModel_Id1 })
                .ForeignKey("dbo.RunningActivityModels", t => t.RunningActivityModel_Id)
                .ForeignKey("dbo.RunningActivityModels", t => t.RunningActivityModel_Id1)
                .Index(t => t.RunningActivityModel_Id)
                .Index(t => t.RunningActivityModel_Id1);
            
            CreateTable(
                "dbo.StateMachineModelRunningActivityModels",
                c => new
                    {
                        StateMachineModel_Id = c.Guid(nullable: false),
                        RunningActivityModel_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.StateMachineModel_Id, t.RunningActivityModel_Id })
                .ForeignKey("dbo.StateMachineModels", t => t.StateMachineModel_Id, cascadeDelete: true)
                .ForeignKey("dbo.RunningActivityModels", t => t.RunningActivityModel_Id, cascadeDelete: true)
                .Index(t => t.StateMachineModel_Id)
                .Index(t => t.RunningActivityModel_Id);
            
            CreateTable(
                "dbo.StateMachineTemplateModelRunningActivityModels",
                c => new
                    {
                        StateMachineTemplateModel_Version = c.Guid(nullable: false),
                        RunningActivityModel_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.StateMachineTemplateModel_Version, t.RunningActivityModel_Id })
                .ForeignKey("dbo.StateMachineTemplateModels", t => t.StateMachineTemplateModel_Version, cascadeDelete: true)
                .ForeignKey("dbo.RunningActivityModels", t => t.RunningActivityModel_Id, cascadeDelete: true)
                .Index(t => t.StateMachineTemplateModel_Version)
                .Index(t => t.RunningActivityModel_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StateMachineModels", "TemplateModel_Version", "dbo.StateMachineTemplateModels");
            DropForeignKey("dbo.StateMachineTemplateModels", "StartActivityModel_Id", "dbo.RunningActivityModels");
            DropForeignKey("dbo.StateMachineTemplateModelRunningActivityModels", "RunningActivityModel_Id", "dbo.RunningActivityModels");
            DropForeignKey("dbo.StateMachineTemplateModelRunningActivityModels", "StateMachineTemplateModel_Version", "dbo.StateMachineTemplateModels");
            DropForeignKey("dbo.StateMachineModelRunningActivityModels", "RunningActivityModel_Id", "dbo.RunningActivityModels");
            DropForeignKey("dbo.StateMachineModelRunningActivityModels", "StateMachineModel_Id", "dbo.StateMachineModels");
            DropForeignKey("dbo.RunningActivityModelRunningActivityModels", "RunningActivityModel_Id1", "dbo.RunningActivityModels");
            DropForeignKey("dbo.RunningActivityModelRunningActivityModels", "RunningActivityModel_Id", "dbo.RunningActivityModels");
            DropIndex("dbo.StateMachineTemplateModelRunningActivityModels", new[] { "RunningActivityModel_Id" });
            DropIndex("dbo.StateMachineTemplateModelRunningActivityModels", new[] { "StateMachineTemplateModel_Version" });
            DropIndex("dbo.StateMachineModelRunningActivityModels", new[] { "RunningActivityModel_Id" });
            DropIndex("dbo.StateMachineModelRunningActivityModels", new[] { "StateMachineModel_Id" });
            DropIndex("dbo.RunningActivityModelRunningActivityModels", new[] { "RunningActivityModel_Id1" });
            DropIndex("dbo.RunningActivityModelRunningActivityModels", new[] { "RunningActivityModel_Id" });
            DropIndex("dbo.StateMachineTemplateModels", new[] { "StartActivityModel_Id" });
            DropIndex("dbo.StateMachineModels", new[] { "TemplateModel_Version" });
            DropTable("dbo.StateMachineTemplateModelRunningActivityModels");
            DropTable("dbo.StateMachineModelRunningActivityModels");
            DropTable("dbo.RunningActivityModelRunningActivityModels");
            DropTable("dbo.StateMachineTemplateModels");
            DropTable("dbo.StateMachineModels");
            DropTable("dbo.RunningActivityModels");
        }
    }
}
