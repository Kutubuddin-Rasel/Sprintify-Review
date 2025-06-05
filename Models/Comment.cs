using Sprintify.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sprintify.Models
{
	public class Comment
	{

		[Key]
		public int CommentId { get; set; }

		[Required]
		public int IssueId { get; set; }
		public virtual Issue Issue { get; set; }

		[Required]
		public int UserId { get; set; }                        // FK → User
		public virtual User Author { get; set; }

		[Required]
		public string Body { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}