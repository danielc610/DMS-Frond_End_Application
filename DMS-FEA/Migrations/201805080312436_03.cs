namespace DMS_FEA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _03 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OUSRs", "ODEP_DocNum1", "dbo.ODEPs");
            DropIndex("dbo.OUSRs", new[] { "ODEP_DocNum1" });
            CreateIndex("dbo.OUSRs", "Department_Code");
            AddForeignKey("dbo.OUSRs", "Department_Code", "dbo.OFOTs", "DocNum");
            DropColumn("dbo.OUSRs", "ODEP_DocNum1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OUSRs", "ODEP_DocNum1", c => c.Int());
            DropForeignKey("dbo.OUSRs", "Department_Code", "dbo.OFOTs");
            DropIndex("dbo.OUSRs", new[] { "Department_Code" });
            CreateIndex("dbo.OUSRs", "ODEP_DocNum1");
            AddForeignKey("dbo.OUSRs", "ODEP_DocNum1", "dbo.ODEPs", "DocNum");
        }
    }
}
