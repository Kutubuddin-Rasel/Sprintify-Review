namespace Sprintify.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePartB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UserName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "UserName");
        }
    }
}
