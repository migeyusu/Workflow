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
                        RunningActivityModel_Id = c.Guid(),
                        StateMachineModel_Id = c.Guid(),
                        StateMachineTemplateModel_Version = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RunningActivityModels", t => t.RunningActivityModel_Id)
                .ForeignKey("dbo.StateMachineModels", t => t.StateMachineModel_Id)
                .ForeignKey("dbo.StateMachineTemplateModels", t => t.StateMachineTemplateModel_Version)
                .Index(t => t.RunningActivityModel_Id)
                .Index(t => t.StateMachineModel_Id)
                .Index(t => t.StateMachineTemplateModel_Version);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StateMachineModels", "TemplateModel_Version", "dbo.StateMachineTemplateModels");
            DropForeignKey("dbo.StateMachineTemplateModels", "StartActivityModel_Id", "dbo.RunningActivityModels");
            DropForeignKey("dbo.RunningActivityModels", "StateMachineTemplateModel_Version", "dbo.StateMachineTemplateModels");
            DropForeignKey("dbo.RunningActivityModels", "StateMachineModel_Id", "dbo.StateMachineModels");
            DropForeignKey("dbo.RunningActivityModels", "RunningActivityModel_Id", "dbo.RunningActivityModels");
            DropIndex("dbo.StateMachineTemplateModels", new[] { "StartActivityModel_Id" });
            DropIndex("dbo.StateMachineModels", new[] { "TemplateModel_Version" });
            DropIndex("dbo.RunningActivityModels", new[] { "StateMachineTemplateModel_Version" });
            DropIndex("dbo.RunningActivityModels", new[] { "StateMachineModel_Id" });
            DropIndex("dbo.RunningActivityModels", new[] { "RunningActivityModel_Id" });
            DropTable("dbo.StateMachineTemplateModels");
            DropTable("dbo.StateMachineModels");
            DropTable("dbo.RunningActivityModels");
        }
    }
}
