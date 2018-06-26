namespace DMS_FEA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OUSRs", "Department_Code", "dbo.ODEPs");
            DropIndex("dbo.OUSRs", new[] { "Department_Code" });
            AddColumn("dbo.OUSRs", "ODEP_DocNum1", c => c.Int());
            CreateIndex("dbo.OUSRs", "ODEP_DocNum1");
            AddForeignKey("dbo.OUSRs", "ODEP_DocNum1", "dbo.ODEPs", "DocNum");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OUSRs", "ODEP_DocNum1", "dbo.ODEPs");
            DropIndex("dbo.OUSRs", new[] { "ODEP_DocNum1" });
            DropColumn("dbo.OUSRs", "ODEP_DocNum1");
            CreateIndex("dbo.OUSRs", "Department_Code");
            AddForeignKey("dbo.OUSRs", "Department_Code", "dbo.ODEPs", "DocNum");
        }
    }
}
