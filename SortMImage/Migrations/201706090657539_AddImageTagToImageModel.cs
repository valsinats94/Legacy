namespace SortMImage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageTagToImageModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ImageTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Confidence = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tag = c.String(),
                        ImageModel_ImageId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ImageModels", t => t.ImageModel_ImageId)
                .Index(t => t.ImageModel_ImageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ImageTags", "ImageModel_ImageId", "dbo.ImageModels");
            DropIndex("dbo.ImageTags", new[] { "ImageModel_ImageId" });
            DropTable("dbo.ImageTags");
        }
    }
}
