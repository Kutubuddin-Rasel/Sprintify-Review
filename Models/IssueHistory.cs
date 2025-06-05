using Sprintify.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sprintify.Models
{
	public class IssueHistory
	{

		[Key]
		public int HistoryId { get; set; }

		[Required]
		public int IssueId { get; set; }
		public virtual Issue Issue { get; set; }

		[Required]
		[StringLength(100)]
		public string FieldName { get; set; }   // e.g. "Status", "Summary", "Priority"

		public string OldValue { get; set; }
		public string NewValue { get; set; }

		[Required]
		public int ChangedById { get; set; }    // FK → User
		public virtual User ChangedBy { get; set; }

		[Required]
		public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
	}
}