namespace DataManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrate3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileModels",
                c => new
                    {
                        token = c.String(nullable: false, maxLength: 128),
                        sharingDuration = c.DateTime(nullable: false),
                        fileName = c.String(nullable: false),
                        fileDuration = c.DateTime(nullable: false),
                        username = c.String(nullable: false, maxLength: 128),
                        url = c.String(nullable: false),
                        type = c.String(nullable: false),
                        size = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.token)
                .ForeignKey("dbo.UserModels", t => t.username, cascadeDelete: true)
                .Index(t => t.username);
            
            CreateTable(
                "dbo.UserModels",
                c => new
                    {
                        username = c.String(nullable: false, maxLength: 128),
                        password = c.String(nullable: false),
                        fname = c.String(nullable: false),
                        lname = c.String(nullable: false),
                        dob = c.DateTime(nullable: false),
                        email = c.String(nullable: false),
                        lastloggedin = c.DateTime(nullable: false),
                        lastPassChange = c.DateTime(nullable: false),
                        usertype = c.String(),
                        isActive = c.Boolean(nullable: false),
                        isVerified = c.Boolean(nullable: false),
                        profilePic = c.String(),
                    })
                .PrimaryKey(t => t.username);
            
            CreateTable(
                "dbo.GroupFileSharingModels",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        token = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.id, t.token })
                .ForeignKey("dbo.FileModels", t => t.token, cascadeDelete: true)
                .ForeignKey("dbo.GroupModels", t => t.id, cascadeDelete: true)
                .Index(t => t.id)
                .Index(t => t.token);
            
            CreateTable(
                "dbo.GroupModels",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        groupName = c.String(nullable: false),
                        owner = c.String(nullable: false),
                        reqPending = c.Int(nullable: false),
                        user_username = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.UserModels", t => t.user_username)
                .Index(t => t.user_username);
            
            CreateTable(
                "dbo.GroupMemberModels",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        username = c.String(nullable: false, maxLength: 128),
                        reqStatus = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.id, t.username })
                .ForeignKey("dbo.GroupModels", t => t.id, cascadeDelete: true)
                .ForeignKey("dbo.UserModels", t => t.username, cascadeDelete: true)
                .Index(t => t.id)
                .Index(t => t.username);
            
            CreateTable(
                "dbo.PlanModels",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        price = c.Double(nullable: false),
                        storageBenefit = c.Double(nullable: false),
                        validity = c.Int(nullable: false),
                        type = c.String(nullable: false),
                        description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ResetPasswordModels",
                c => new
                    {
                        token = c.String(nullable: false, maxLength: 128),
                        username = c.String(nullable: false, maxLength: 128),
                        gen_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.token)
                .ForeignKey("dbo.UserModels", t => t.username, cascadeDelete: true)
                .Index(t => t.username);
            
            CreateTable(
                "dbo.TokenUserModels",
                c => new
                    {
                        username = c.String(nullable: false, maxLength: 128),
                        password = c.String(nullable: false),
                        name = c.String(nullable: false),
                        type = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.username);
            
            CreateTable(
                "dbo.UserPlanModels",
                c => new
                    {
                        id = c.Int(nullable: false),
                        username = c.String(nullable: false, maxLength: 128),
                        subTime = c.DateTime(nullable: false),
                        expiryTime = c.DateTime(nullable: false),
                        storageRemaining = c.Double(nullable: false),
                        priority = c.Int(),
                    })
                .PrimaryKey(t => new { t.id, t.username })
                .ForeignKey("dbo.PlanModels", t => t.id, cascadeDelete: true)
                .ForeignKey("dbo.UserModels", t => t.username, cascadeDelete: true)
                .Index(t => t.id)
                .Index(t => t.username);
            
            CreateTable(
                "dbo.VerifyAccountModels",
                c => new
                    {
                        token = c.String(nullable: false, maxLength: 128),
                        username = c.String(nullable: false, maxLength: 128),
                        gen_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.token)
                .ForeignKey("dbo.UserModels", t => t.username, cascadeDelete: true)
                .Index(t => t.username);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VerifyAccountModels", "username", "dbo.UserModels");
            DropForeignKey("dbo.UserPlanModels", "username", "dbo.UserModels");
            DropForeignKey("dbo.UserPlanModels", "id", "dbo.PlanModels");
            DropForeignKey("dbo.ResetPasswordModels", "username", "dbo.UserModels");
            DropForeignKey("dbo.GroupFileSharingModels", "id", "dbo.GroupModels");
            DropForeignKey("dbo.GroupModels", "user_username", "dbo.UserModels");
            DropForeignKey("dbo.GroupMemberModels", "username", "dbo.UserModels");
            DropForeignKey("dbo.GroupMemberModels", "id", "dbo.GroupModels");
            DropForeignKey("dbo.GroupFileSharingModels", "token", "dbo.FileModels");
            DropForeignKey("dbo.FileModels", "username", "dbo.UserModels");
            DropIndex("dbo.VerifyAccountModels", new[] { "username" });
            DropIndex("dbo.UserPlanModels", new[] { "username" });
            DropIndex("dbo.UserPlanModels", new[] { "id" });
            DropIndex("dbo.ResetPasswordModels", new[] { "username" });
            DropIndex("dbo.GroupMemberModels", new[] { "username" });
            DropIndex("dbo.GroupMemberModels", new[] { "id" });
            DropIndex("dbo.GroupModels", new[] { "user_username" });
            DropIndex("dbo.GroupFileSharingModels", new[] { "token" });
            DropIndex("dbo.GroupFileSharingModels", new[] { "id" });
            DropIndex("dbo.FileModels", new[] { "username" });
            DropTable("dbo.VerifyAccountModels");
            DropTable("dbo.UserPlanModels");
            DropTable("dbo.TokenUserModels");
            DropTable("dbo.ResetPasswordModels");
            DropTable("dbo.PlanModels");
            DropTable("dbo.GroupMemberModels");
            DropTable("dbo.GroupModels");
            DropTable("dbo.GroupFileSharingModels");
            DropTable("dbo.UserModels");
            DropTable("dbo.FileModels");
        }
    }
}
