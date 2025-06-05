using Sprintify.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sprintify.Models
{
	public class Project
	{
		[Key]
		public int ProjectId { get; set; }

		[Required]
		[StringLength(10)]
		public string Key { get; set; }            // e.g. "SPR"

		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		// FK to User: project lead (stub for now)
		[Required]
		public int LeadUserId { get; set; }
		public virtual User Lead { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Navigation collections:
		public virtual ICollection<Issue> Issues { get; set; }
		public virtual ICollection<Sprint> Sprints { get; set; }
		public virtual ICollection<BoardColumn> Columns { get; set; }

	}
}