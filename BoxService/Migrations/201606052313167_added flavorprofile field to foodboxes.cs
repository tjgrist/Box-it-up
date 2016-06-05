namespace BoxService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedflavorprofilefieldtofoodboxes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FoodBoxes", "FlavorProfile", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FoodBoxes", "FlavorProfile");
        }
    }
}
