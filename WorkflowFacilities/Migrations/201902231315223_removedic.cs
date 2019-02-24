namespace WorkflowFacilities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedic : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StateMachineModels", "LocalVariousDictionary");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StateMachineModels", "LocalVariousDictionary", c => c.String());
        }
    }
}
