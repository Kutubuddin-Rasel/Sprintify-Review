namespace Sprintify.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSprintifySchema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attachments",
                c => new
                    {
                        AttachmentId = c.Int(nullable: false, identity: true),
                        IssueId = c.Int(nullable: false),
                        FileName = c.String(nullable: false, maxLength: 200),
                        FilePath = c.String(nullable: false, maxLength: 500),
                        UploadedById = c.Int(nullable: false),
                        UploadedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AttachmentId)
                .ForeignKey("dbo.Issues", t => t.IssueId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UploadedById)
                .Index(t => t.IssueId)
                .Index(t => t.UploadedById);
            
            CreateTable(
                "dbo.Issues",
                c => new
                    {
                        IssueId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        IssueType = c.String(nullable: false, maxLength: 20),
                        Key = c.String(nullable: false, maxLength: 20),
                        Summary = c.String(nullable: false, maxLength: 500),
                        Description = c.String(),
                        Priority = c.String(maxLength: 20),
                        Status = c.String(maxLength: 20),
                        AssigneeId = c.Int(),
                        ReporterId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.IssueId)
                .ForeignKey("dbo.Users", t => t.AssigneeId)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .ForeignKey("dbo.Users", t => t.ReporterId)
                .Index(t => t.ProjectId)
                .Index(t => t.AssigneeId)
                .Index(t => t.ReporterId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        IssueId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Body = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Issues", t => t.IssueId, cascadeDelete: true)
                .Index(t => t.IssueId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.IssueHistories",
                c => new
                    {
                        HistoryId = c.Int(nullable: false, identity: true),
                        IssueId = c.Int(nullable: false),
                        FieldName = c.String(nullable: false, maxLength: 100),
                        OldValue = c.String(),
                        NewValue = c.String(),
                        ChangedById = c.Int(nullable: false),
                        ChangedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.HistoryId)
                .ForeignKey("dbo.Users", t => t.ChangedById)
                .ForeignKey("dbo.Issues", t => t.IssueId, cascadeDelete: true)
                .Index(t => t.IssueId)
                .Index(t => t.ChangedById);
            
            CreateTable(
                "dbo.IssueLabels",
                c => new
                    {
                        IssueId = c.Int(nullable: false),
                        Label = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => new { t.IssueId, t.Label })
                .ForeignKey("dbo.Issues", t => t.IssueId, cascadeDelete: true)
                .Index(t => t.IssueId);
            
            CreateTable(
                "dbo.IssueLinks",
                c => new
                    {
                        IssueLinkId = c.Int(nullable: false, identity: true),
                        SourceIssueId = c.Int(nullable: false),
                        TargetIssueId = c.Int(nullable: false),
                        LinkType = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.IssueLinkId)
                .ForeignKey("dbo.Issues", t => t.SourceIssueId)
                .ForeignKey("dbo.Issues", t => t.TargetIssueId)
                .Index(t => t.SourceIssueId)
                .Index(t => t.TargetIssueId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false, maxLength: 10),
                        Name = c.String(nullable: false, maxLength: 200),
                        LeadUserId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectId)
                .ForeignKey("dbo.Users", t => t.LeadUserId, cascadeDelete: true)
                .Index(t => t.LeadUserId);
            
            CreateTable(
                "dbo.BoardColumns",
                c => new
                    {
                        ColumnId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Status = c.String(nullable: false, maxLength: 20),
                        WIPLimit = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ColumnId)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Sprints",
                c => new
                    {
                        SprintId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        Goal = c.String(),
                        Status = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.SprintId)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.SprintIssues",
                c => new
                    {
                        SprintId = c.Int(nullable: false),
                        IssueId = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SprintId, t.IssueId })
                .ForeignKey("dbo.Issues", t => t.IssueId)
                .ForeignKey("dbo.Sprints", t => t.SprintId, cascadeDelete: true)
                .Index(t => t.SprintId)
                .Index(t => t.IssueId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Attachments", "UploadedById", "dbo.Users");
            DropForeignKey("dbo.Attachments", "IssueId", "dbo.Issues");
            DropForeignKey("dbo.Issues", "ReporterId", "dbo.Users");
            DropForeignKey("dbo.Issues", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.SprintIssues", "SprintId", "dbo.Sprints");
            DropForeignKey("dbo.SprintIssues", "IssueId", "dbo.Issues");
            DropForeignKey("dbo.Sprints", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Projects", "LeadUserId", "dbo.Users");
            DropForeignKey("dbo.BoardColumns", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.IssueLinks", "TargetIssueId", "dbo.Issues");
            DropForeignKey("dbo.IssueLinks", "SourceIssueId", "dbo.Issues");
            DropForeignKey("dbo.IssueLabels", "IssueId", "dbo.Issues");
            DropForeignKey("dbo.IssueHistories", "IssueId", "dbo.Issues");
            DropForeignKey("dbo.IssueHistories", "ChangedById", "dbo.Users");
            DropForeignKey("dbo.Comments", "IssueId", "dbo.Issues");
            DropForeignKey("dbo.Comments", "UserId", "dbo.Users");
            DropForeignKey("dbo.Issues", "AssigneeId", "dbo.Users");
            DropIndex("dbo.SprintIssues", new[] { "IssueId" });
            DropIndex("dbo.SprintIssues", new[] { "SprintId" });
            DropIndex("dbo.Sprints", new[] { "ProjectId" });
            DropIndex("dbo.BoardColumns", new[] { "ProjectId" });
            DropIndex("dbo.Projects", new[] { "LeadUserId" });
            DropIndex("dbo.IssueLinks", new[] { "TargetIssueId" });
            DropIndex("dbo.IssueLinks", new[] { "SourceIssueId" });
            DropIndex("dbo.IssueLabels", new[] { "IssueId" });
            DropIndex("dbo.IssueHistories", new[] { "ChangedById" });
            DropIndex("dbo.IssueHistories", new[] { "IssueId" });
            DropIndex("dbo.Comments", new[] { "UserId" });
            DropIndex("dbo.Comments", new[] { "IssueId" });
            DropIndex("dbo.Issues", new[] { "ReporterId" });
            DropIndex("dbo.Issues", new[] { "AssigneeId" });
            DropIndex("dbo.Issues", new[] { "ProjectId" });
            DropIndex("dbo.Attachments", new[] { "UploadedById" });
            DropIndex("dbo.Attachments", new[] { "IssueId" });
            DropTable("dbo.SprintIssues");
            DropTable("dbo.Sprints");
            DropTable("dbo.BoardColumns");
            DropTable("dbo.Projects");
            DropTable("dbo.IssueLinks");
            DropTable("dbo.IssueLabels");
            DropTable("dbo.IssueHistories");
            DropTable("dbo.Comments");
            DropTable("dbo.Users");
            DropTable("dbo.Issues");
            DropTable("dbo.Attachments");
        }
    }
}
