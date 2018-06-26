namespace DMS_FEA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _05 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OFOTs", "Fparent", "dbo.OFOTs");
            DropIndex("dbo.OFOTs", new[] { "Fparent" });
            DropColumn("dbo.OFOTs", "ShortName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OFOTs", "ShortName", c => c.String(maxLength: 15));
            CreateIndex("dbo.OFOTs", "Fparent");
            AddForeignKey("dbo.OFOTs", "Fparent", "dbo.OFOTs", "DocNum");
        }
    }
}
