namespace WorkflowFacilities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removebookmark : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RunningActivityModels", "Bookmark");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RunningActivityModels", "Bookmark", c => c.String());
        }
    }
}
