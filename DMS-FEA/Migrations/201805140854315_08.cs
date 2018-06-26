namespace DMS_FEA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _08 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OFOTs", "deptID", c => c.Int());
            CreateIndex("dbo.OFOTs", "deptID");
            AddForeignKey("dbo.OFOTs", "deptID", "dbo.ODEPs", "DocNum");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OFOTs", "deptID", "dbo.ODEPs");
            DropIndex("dbo.OFOTs", new[] { "deptID" });
            DropColumn("dbo.OFOTs", "deptID");
        }
    }
}
