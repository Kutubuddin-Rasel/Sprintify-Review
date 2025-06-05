using Sprintify.Models.Auth;
using Sprintify.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace Sprintify.Context
{
	public class AppDbContext : DbContext
	{
		// Your context has been configured to use a 'AppDbContext' connection string from your application's 
		// configuration file (App.config or Web.config). By default, this connection string targets the 
		// 'Sprintify.Context.AppDbContext' database on your LocalDb instance. 
		// 
		// If you wish to target a different database and/or database provider, modify the 'AppDbContext' 
		// connection string in the application configuration file.
		public AppDbContext()
			: base("name=AppDbContext")
		{
		}

		// Add a DbSet for each entity type that you want to include in your model. For more information 
		// on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

		// public virtual DbSet<MyEntity> MyEntities { get; set; }
		
		public DbSet<User> Users { get; set; }

		public DbSet<Project> Projects { get; set; }
		public DbSet<Issue> Issues { get; set; }
		public DbSet<IssueLink> IssueLinks { get; set; }
		public DbSet<IssueLabel> IssueLabels { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Attachment> Attachments { get; set; }
		public DbSet<IssueHistory> IssueHistories { get; set; }
		public DbSet<Sprint> Sprints { get; set; }
		public DbSet<SprintIssue> SprintIssues { get; set; }
		public DbSet<BoardColumn> BoardColumns { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Composite key for IssueLabel
			modelBuilder.Entity<IssueLabel>()
				.HasKey(il => new { il.IssueId, il.Label });

			// Composite key for SprintIssue
			modelBuilder.Entity<SprintIssue>()
				.HasKey(si => new { si.SprintId, si.IssueId });

			// Project ? Issue (1 : M)
			modelBuilder.Entity<Issue>()
				.HasRequired(i => i.Project)
				.WithMany(p => p.Issues)
				.HasForeignKey(i => i.ProjectId)
				.WillCascadeOnDelete(false);

			// Issue ? Assignee (optional)
			modelBuilder.Entity<Issue>()
				.HasOptional(i => i.Assignee)
				.WithMany()
				.HasForeignKey(i => i.AssigneeId)
				.WillCascadeOnDelete(false);

			// Issue ? Reporter (required)
			modelBuilder.Entity<Issue>()
				.HasRequired(i => i.Reporter)
				.WithMany()
				.HasForeignKey(i => i.ReporterId)
				.WillCascadeOnDelete(false);

			// IssueLink relationships
			modelBuilder.Entity<IssueLink>()
				.HasRequired(il => il.Source)
				.WithMany(i => i.LinksFrom)
				.HasForeignKey(il => il.SourceIssueId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<IssueLink>()
				.HasRequired(il => il.Target)
				.WithMany(i => i.LinksTo)
				.HasForeignKey(il => il.TargetIssueId)
				.WillCascadeOnDelete(false);

			// IssueLabel ? Issue
			modelBuilder.Entity<IssueLabel>()
				.HasRequired(il => il.Issue)
				.WithMany(i => i.Labels)
				.HasForeignKey(il => il.IssueId)
				.WillCascadeOnDelete(true);

			// Comment ? Issue & Comment ? User
			modelBuilder.Entity<Comment>()
				.HasRequired(c => c.Issue)
				.WithMany(i => i.Comments)
				.HasForeignKey(c => c.IssueId)
				.WillCascadeOnDelete(true);

			modelBuilder.Entity<Comment>()
				.HasRequired(c => c.Author)
				.WithMany()
				.HasForeignKey(c => c.UserId)
				.WillCascadeOnDelete(false);

			// Attachment ? Issue & Attachment ? User
			modelBuilder.Entity<Attachment>()
				.HasRequired(a => a.Issue)
				.WithMany(i => i.Attachments)
				.HasForeignKey(a => a.IssueId)
				.WillCascadeOnDelete(true);

			modelBuilder.Entity<Attachment>()
				.HasRequired(a => a.Uploader)
				.WithMany()
				.HasForeignKey(a => a.UploadedById)
				.WillCascadeOnDelete(false);

			// IssueHistory ? Issue & IssueHistory ? User
			modelBuilder.Entity<IssueHistory>()
				.HasRequired(h => h.Issue)
				.WithMany(i => i.HistoryEntries)
				.HasForeignKey(h => h.IssueId)
				.WillCascadeOnDelete(true);

			modelBuilder.Entity<IssueHistory>()
				.HasRequired(h => h.ChangedBy)
				.WithMany()
				.HasForeignKey(h => h.ChangedById)
				.WillCascadeOnDelete(false);

			// Sprint ? Project
			modelBuilder.Entity<Sprint>()
				.HasRequired(s => s.Project)
				.WithMany(p => p.Sprints)
				.HasForeignKey(s => s.ProjectId)
				.WillCascadeOnDelete(false);

			// SprintIssue relationships
			modelBuilder.Entity<SprintIssue>()
				.HasRequired(si => si.Sprint)
				.WithMany(s => s.SprintIssues)
				.HasForeignKey(si => si.SprintId)
				.WillCascadeOnDelete(true);

			modelBuilder.Entity<SprintIssue>()
				.HasRequired(si => si.Issue)
				.WithMany(i => i.SprintIssues)
				.HasForeignKey(si => si.IssueId)
				.WillCascadeOnDelete(false);

			// BoardColumn ? Project
			modelBuilder.Entity<BoardColumn>()
				.HasRequired(bc => bc.Project)
				.WithMany(p => p.Columns)
				.HasForeignKey(bc => bc.ProjectId)
				.WillCascadeOnDelete(true);
		}
	}

	//public class MyEntity
	//{
	//    public int Id { get; set; }
	//    public string Name { get; set; }
	//}
}