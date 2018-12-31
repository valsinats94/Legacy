namespace SortMImage.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsProcessedAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImageModels", "IsProcessed", c => c.Boolean(false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ImageModels", "IsProcessed");
        }
    }
}
