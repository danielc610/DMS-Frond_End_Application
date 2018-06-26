namespace DMS_FEA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _06 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUSRs", "UserRole", c => c.String(maxLength: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OUSRs", "UserRole");
        }
    }
}
