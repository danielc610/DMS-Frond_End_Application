namespace DMS_FEA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OCOMs", "ShortName", c => c.String(maxLength: 15));
            AddColumn("dbo.OFOTs", "ShortName", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OFOTs", "ShortName");
            DropColumn("dbo.OCOMs", "ShortName");
        }
    }
}
