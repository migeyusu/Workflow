namespace WorkflowFacilities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReviseP : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StateMachineTemplateModels", "CodeTemplateName", c => c.String());
            DropColumn("dbo.StateMachineTemplateModels", "TemplateClassTypeName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StateMachineTemplateModels", "TemplateClassTypeName", c => c.String());
            DropColumn("dbo.StateMachineTemplateModels", "CodeTemplateName");
        }
    }
}
