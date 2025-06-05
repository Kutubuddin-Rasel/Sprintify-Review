using Sprintify.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Sprintify.Models
{
	public class Issue
	{

		[Key]
		public int IssueId { get; set; }

		// FK to Project (required)
		[Required]
		public int ProjectId { get; set; }
		public virtual Project Project { get; set; }

		[Required]
		[StringLength(20)]
		public string IssueType { get; set; }     // e.g. "Story", "Bug", "Task"

		[Required]
		[StringLength(20)]
		public string Key { get; set; }           // e.g. "SPR-101"

		[Required]
		[StringLength(500)]
		public string Summary { get; set; }

		public string Description { get; set; }

		[StringLength(20)]
		public string Priority { get; set; }      // "High", "Medium", "Low"

		[StringLength(20)]
		public string Status { get; set; }        // "To Do", "In Progress", "Done"

		// Assignee (nullable)
		public int? AssigneeId { get; set; }
		public virtual User Assignee { get; set; }

		// Reporter (required)
		[Required]
		public int ReporterId { get; set; }
		public virtual User Reporter { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// Concurrency token (to prevent lost updates)
		[Timestamp]
		public byte[] RowVersion { get; set; }

		// Navigation collections:
		public virtual ICollection<IssueLink> LinksFrom { get; set; }
		public virtual ICollection<IssueLink> LinksTo { get; set; }
		public virtual ICollection<IssueLabel> Labels { get; set; }
		public virtual ICollection<Comment> Comments { get; set; }
		public virtual ICollection<Attachment> Attachments { get; set; }
		public virtual ICollection<IssueHistory> HistoryEntries { get; set; }
		public virtual ICollection<SprintIssue> SprintIssues { get; set; }
	}
}