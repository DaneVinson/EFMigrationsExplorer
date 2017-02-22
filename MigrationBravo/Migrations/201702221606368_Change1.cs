namespace MigrationsLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dodads", "NewField1", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dodads", "NewField1");
        }
    }
}
