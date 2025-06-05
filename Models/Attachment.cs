using Sprintify.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sprintify.Models
{
	public class Attachment
	{

		[Key]
		public int AttachmentId { get; set; }

		[Required]
		public int IssueId { get; set; }
		public virtual Issue Issue { get; set; }

		[Required]
		[StringLength(200)]
		public string FileName { get; set; }

		[Required]
		[StringLength(500)]
		public string FilePath { get; set; }  // e.g. "/uploads/issue_101_log.txt"

		[Required]
		public int UploadedById { get; set; }
		public virtual User Uploader { get; set; }

		[Required]
		public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
	}
}