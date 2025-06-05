using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sprintify.Models
{
	public class Sprint
	{

		[Key]
		public int SprintId { get; set; }

		[Required]
		public int ProjectId { get; set; }
		public virtual Project Project { get; set; }

		[Required]
		[StringLength(200)]
		public string Name { get; set; }

		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public string Goal { get; set; }

		[Required]
		[StringLength(20)]
		public string Status { get; set; }   // "Planned", "Active", "Completed"

		public virtual ICollection<SprintIssue> SprintIssues { get; set; }
	}
}