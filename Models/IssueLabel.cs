using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sprintify.Models
{
	public class IssueLabel
	{

		// Composite key: (IssueId, Label)
		[Key, Column(Order = 0)]
		public int IssueId { get; set; }
		public virtual Issue Issue { get; set; }

		[Key, Column(Order = 1)]
		[StringLength(50)]
		public string Label { get; set; }   // e.g. "frontend", "backend", "urgent"
	}
}