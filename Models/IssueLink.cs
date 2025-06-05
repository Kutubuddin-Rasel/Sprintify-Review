using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sprintify.Models
{
	public class IssueLink
	{

		[Key]
		public int IssueLinkId { get; set; }

		// The issue that "owns" this link
		[Required]
		public int SourceIssueId { get; set; }
		public virtual Issue Source { get; set; }

		// The linked issue
		[Required]
		public int TargetIssueId { get; set; }
		public virtual Issue Target { get; set; }

		// e.g. "Blocks", "Relates", "Duplicate"
		[Required]
		[StringLength(20)]
		public string LinkType { get; set; }
	}
}