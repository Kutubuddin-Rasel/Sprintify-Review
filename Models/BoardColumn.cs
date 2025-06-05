using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sprintify.Models
{
	public class BoardColumn
	{
		[Key]
		public int ColumnId { get; set; }

		[Required]
		public int ProjectId { get; set; }
		public virtual Project Project { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }     // e.g. "To Do", "In Progress", "Done"

		[Required]
		[StringLength(20)]
		public string Status { get; set; }   // must match Issue.Status when showing cards here

		public int WIPLimit { get; set; }    // e.g. maximum number of cards allowed
	}
}