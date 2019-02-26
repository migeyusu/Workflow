namespace WorkflowFacilities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsuspend : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SuspendedRunningActivityModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BookmarkName = c.String(),
                        RunningActivityModel_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RunningActivityModels", t => t.RunningActivityModel_Id)
                .Index(t => t.RunningActivityModel_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SuspendedRunningActivityModels", "RunningActivityModel_Id", "dbo.RunningActivityModels");
            DropIndex("dbo.SuspendedRunningActivityModels", new[] { "RunningActivityModel_Id" });
            DropTable("dbo.SuspendedRunningActivityModels");
        }
    }
}
