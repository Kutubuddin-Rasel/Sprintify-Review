using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sprintify.Models
{
	public class SprintIssue
	{
		[Key, Column(Order = 0)]
		public int SprintId { get; set; }
		public virtual Sprint Sprint { get; set; }

		[Key, Column(Order = 1)]
		public int IssueId { get; set; }
		public virtual Issue Issue { get; set; }

		public int Rank { get; set; }   // position/order within the sprint/backlog
	}
}