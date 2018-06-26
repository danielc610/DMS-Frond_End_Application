namespace DMS_FEA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OCOMs",
                c => new
                    {
                        DocNum = c.Int(nullable: false, identity: true),
                        Comp_Code = c.String(nullable: false),
                        Comp_Name = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(),
                        CreateSign = c.Int(),
                        UpdateDate = c.DateTime(),
                        UpdateSign = c.Int(),
                    })
                .PrimaryKey(t => t.DocNum);

            CreateTable(
                "dbo.OUSRs",
                c => new
                {
                    DocNum = c.Int(nullable: false, identity: true),
                    User_Code = c.String(nullable: false, maxLength: 25),
                    Password = c.String(nullable: false),
                    Active = c.Boolean(nullable: false),
                    Company_Code = c.Int(),
                    Department_Code = c.Int(),
                    U_Name = c.String(maxLength: 155),
                    Nick_Name = c.String(maxLength: 50),
                    UserRold = c.Boolean(),
                        Position = c.String(maxLength: 90),
                        E_Mail = c.String(),
                        CreateDate = c.DateTime(),
                        CreateSign = c.Int(),
                        UpdateDate = c.DateTime(),
                        UpdateSign = c.Int(),
                        LstPwdCht = c.DateTime(),
                        ODEP_DocNum = c.Int(),
                    })
                .PrimaryKey(t => t.DocNum)
                .ForeignKey("dbo.OCOMs", t => t.Company_Code)
                .ForeignKey("dbo.ODEPs", t => t.ODEP_DocNum)
                .ForeignKey("dbo.ODEPs", t => t.Department_Code)
                .Index(t => t.Company_Code)
                .Index(t => t.Department_Code)
                .Index(t => t.ODEP_DocNum);
            
            CreateTable(
                "dbo.ODEPs",
                c => new
                    {
                        DocNum = c.Int(nullable: false, identity: true),
                        Dept_Code = c.String(nullable: false),
                        Dept_Name = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(),
                        CreateSign = c.Int(),
                        UpdateDate = c.DateTime(),
                        UpdateSign = c.Int(),
                        OUSR_DocNum = c.Int(),
                    })
                .PrimaryKey(t => t.DocNum)
                .ForeignKey("dbo.OUSRs", t => t.OUSR_DocNum)
                .Index(t => t.OUSR_DocNum);
            
            CreateTable(
                "dbo.OFOTs",
                c => new
                    {
                        DocNum = c.Int(nullable: false, identity: true),
                        Fparent = c.Int(),
                        Fname = c.String(),
                        Fchild = c.Int(),
                        Flevel = c.Int(),
                        Fauto = c.String(),
                        Fseries = c.String(),
                        Fnumber = c.Int(),
                        compID = c.Int(),
                        Fremarks = c.String(),
                        Active = c.Boolean(nullable: false),
                        Owner = c.Int(),
                        CreateDate = c.DateTime(),
                        CreateSign = c.Int(),
                        UpdateDate = c.DateTime(),
                        UpdateSign = c.Int(),
                    })
                .PrimaryKey(t => t.DocNum)
                .ForeignKey("dbo.OCOMs", t => t.compID)
                .ForeignKey("dbo.OFOTs", t => t.Fparent)
                .Index(t => t.Fparent)
                .Index(t => t.compID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OFOTs", "Fparent", "dbo.OFOTs");
            DropForeignKey("dbo.OFOTs", "compID", "dbo.OCOMs");
            DropForeignKey("dbo.OUSRs", "Department_Code", "dbo.ODEPs");
            DropForeignKey("dbo.OUSRs", "ODEP_DocNum", "dbo.ODEPs");
            DropForeignKey("dbo.ODEPs", "OUSR_DocNum", "dbo.OUSRs");
            DropForeignKey("dbo.OUSRs", "Company_Code", "dbo.OCOMs");
            DropIndex("dbo.OFOTs", new[] { "compID" });
            DropIndex("dbo.OFOTs", new[] { "Fparent" });
            DropIndex("dbo.ODEPs", new[] { "OUSR_DocNum" });
            DropIndex("dbo.OUSRs", new[] { "ODEP_DocNum" });
            DropIndex("dbo.OUSRs", new[] { "Department_Code" });
            DropIndex("dbo.OUSRs", new[] { "Company_Code" });
            DropTable("dbo.OFOTs");
            DropTable("dbo.ODEPs");
            DropTable("dbo.OUSRs");
            DropTable("dbo.OCOMs");
        }
    }
}
