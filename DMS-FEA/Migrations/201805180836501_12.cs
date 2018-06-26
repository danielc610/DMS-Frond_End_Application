namespace DMS_FEA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OCOMs", "Fname", c => c.String(maxLength: 15));
            AddColumn("dbo.ODEPs", "Fname", c => c.String());
            AlterColumn("dbo.OCOMs", "Comp_Code", c => c.String(nullable: false, maxLength: 2));
            DropColumn("dbo.OCOMs", "ShortName");
            DropColumn("dbo.ODEPs", "ShortName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ODEPs", "ShortName", c => c.String());
            AddColumn("dbo.OCOMs", "ShortName", c => c.String(maxLength: 15));
            AlterColumn("dbo.OCOMs", "Comp_Code", c => c.String(nullable: false));
            DropColumn("dbo.ODEPs", "Fname");
            DropColumn("dbo.OCOMs", "Fname");
        }
    }
}
