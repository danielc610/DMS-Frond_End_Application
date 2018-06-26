namespace DMS_FEA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _11 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.OFOTs", "Fparent");
            AddForeignKey("dbo.OFOTs", "Fparent", "dbo.OFOTs", "DocNum");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OFOTs", "Fparent", "dbo.OFOTs");
            DropIndex("dbo.OFOTs", new[] { "Fparent" });
        }
    }
}
