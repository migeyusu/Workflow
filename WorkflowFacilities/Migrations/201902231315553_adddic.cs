namespace WorkflowFacilities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StateMachineModels", "LocalVariousDictionary", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StateMachineModels", "LocalVariousDictionary");
        }
    }
}
