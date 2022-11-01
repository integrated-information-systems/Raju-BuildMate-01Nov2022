namespace BMSS.WebUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class preferredCashSalesCustomeradded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "preferredCashSalesCustomer", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "preferredCashSalesCustomer");
        }
    }
}
