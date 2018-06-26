namespace DMS_FEA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _14 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OFOTs", "Fname", c => c.String(maxLength: 15));
            AlterColumn("dbo.ODEPs", "Dept_Code", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.ODEPs", "Fname", c => c.String(maxLength: 15));
            AlterColumn("dbo.ODEPs", "Dept_Name", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ODEPs", "Dept_Name", c => c.String());
            AlterColumn("dbo.ODEPs", "Fname", c => c.String());
            AlterColumn("dbo.ODEPs", "Dept_Code", c => c.String(nullable: false));
            AlterColumn("dbo.OFOTs", "Fname", c => c.String());
        }
    }
}
