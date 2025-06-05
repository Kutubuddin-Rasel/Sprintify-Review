using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sprintify.Models.Auth
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		[Required]
		public string UserName { get; set; }
	}
}