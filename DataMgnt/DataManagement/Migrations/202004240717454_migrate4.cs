namespace DataManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GroupModels", "user_username", "dbo.UserModels");
            DropIndex("dbo.GroupModels", new[] { "user_username" });
            DropColumn("dbo.GroupModels", "owner");
            RenameColumn(table: "dbo.GroupModels", name: "user_username", newName: "owner");
            AlterColumn("dbo.GroupModels", "owner", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.GroupModels", "owner", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.GroupModels", "owner");
            AddForeignKey("dbo.GroupModels", "owner", "dbo.UserModels", "username", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupModels", "owner", "dbo.UserModels");
            DropIndex("dbo.GroupModels", new[] { "owner" });
            AlterColumn("dbo.GroupModels", "owner", c => c.String(maxLength: 128));
            AlterColumn("dbo.GroupModels", "owner", c => c.String(nullable: false));
            RenameColumn(table: "dbo.GroupModels", name: "owner", newName: "user_username");
            AddColumn("dbo.GroupModels", "owner", c => c.String(nullable: false));
            CreateIndex("dbo.GroupModels", "user_username");
            AddForeignKey("dbo.GroupModels", "user_username", "dbo.UserModels", "username");
        }
    }
}
