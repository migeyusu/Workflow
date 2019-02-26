namespace WorkflowFacilities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeToDisplayName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RunningActivityModels", "DisplayName", c => c.String());
            DropColumn("dbo.RunningActivityModels", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RunningActivityModels", "Name", c => c.String());
            DropColumn("dbo.RunningActivityModels", "DisplayName");
        }
    }
}
