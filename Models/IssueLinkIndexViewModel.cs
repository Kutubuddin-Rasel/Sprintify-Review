using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sprintify.Models
{
	public class IssueLinkIndexViewModel
	{
		public int IssueId {  get; set; }
		public List<IssueLink> OutgoingLinks { get; set; }
		public List<IssueLink> IncomingLinks { get; set; }
	}
}