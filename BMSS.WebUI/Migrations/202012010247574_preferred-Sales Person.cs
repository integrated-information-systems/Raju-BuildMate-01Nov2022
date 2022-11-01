namespace BMSS.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class preferredSalesPerson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "preferredSalesPerson", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "preferredSalesPerson");
        }
    }
}
