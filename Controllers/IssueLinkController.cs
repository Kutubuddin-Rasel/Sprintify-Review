using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sprintify.Context;
using Sprintify.Models;
using Sprintify.Services;

namespace Sprintify.Controllers
{
	public class IssueLinkController : Controller
	{
		private readonly IssueLinkService _service;
		private readonly AppDbContext dbcontext;
		public IssueLinkController()
		{
			dbcontext = new AppDbContext();
			_service = new IssueLinkService();
		}
		public async Task<ActionResult> Index(int issueId)
		{
			var issue = await new IssueService().GetByIdAsync(issueId);
			if (issue == null) return HttpNotFound();

			ViewBag.IssueId = issueId;
			ViewBag.IssueKey = issue.Key;

			var outgoing = await _service.GetLinksBySourceAsync(issueId);
			var incoming = await _service.GetLinksByTargetAsync(issueId);

			var vm = new IssueLinkIndexViewModel
			{
				IssueId = issueId,
				OutgoingLinks = outgoing,
				IncomingLinks = incoming
			};
			return View(vm);
		}

		public async Task<ActionResult> Details(int id)
		{
			var link = await _service.GetByIdAsync(id);
			if (link == null) return HttpNotFound();
			return View(link);
		}

		public async Task<ActionResult> Form(int? id, int issueId)
		{
			var issue = await new IssueService().GetByIdAsync(issueId);
			if (issue == null) return HttpNotFound();

			ViewBag.IssueId = issueId;
			ViewBag.IssueKey = issue.Key;

			var allOther = dbcontext.Issues
							   .Where(i => i.IssueId != issueId)
							   .OrderBy(i => i.Key)
							   .ToList();
			ViewBag.AllIssues = new SelectList(allOther, "IssueId", "Key");

			var linkTypes = new[] { "Blocks", "Relates", "Duplicate" };
			ViewBag.LinkTypes = new SelectList(linkTypes);

			if (id == null || id == 0)
			{
				var newLink = new IssueLink
				{
					SourceIssueId = issueId
				};
				return View(newLink);
			}
			else
			{
				var existing = await _service.GetByIdAsync(id.Value);
				if (existing == null) return HttpNotFound();

				ViewBag.AllIssues = new SelectList(
					allOther, "IssueId", "Key", existing.TargetIssueId
				);
				ViewBag.LinkTypes = new SelectList(linkTypes, existing.LinkType);
				return View(existing);
			}
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Form(
			[Bind(Include = "IssueLinkId,SourceIssueId,TargetIssueId,LinkType")]
			IssueLink link
		)
		{
			var issue = await new IssueService().GetByIdAsync(link.SourceIssueId);
			if (issue == null) return HttpNotFound();

			if (!ModelState.IsValid)
			{
				ViewBag.IssueId = link.SourceIssueId;
				ViewBag.IssueKey = issue.Key;

				var allOther = dbcontext.Issues
								   .Where(i => i.IssueId != link.SourceIssueId)
								   .OrderBy(i => i.Key)
								   .ToList();
				ViewBag.AllIssues = new SelectList(allOther, "IssueId", "Key", link.TargetIssueId);
				ViewBag.LinkTypes = new SelectList(new[] { "Blocks", "Relates", "Duplicate" }, link.LinkType);
				return View(link);
			}

			try
			{
				if (link.IssueLinkId == 0)
				{
					await _service.CreateAsync(link);
				}
				else
				{
					var success = await _service.UpdateAsync(link);
					if (!success) return HttpNotFound();
				}
			}
			catch (InvalidOperationException ex)
			{
				ModelState.AddModelError("", ex.Message);

				ViewBag.IssueId = link.SourceIssueId;
				ViewBag.IssueKey = issue.Key;
				var allOther = dbcontext.Issues
								   .Where(i => i.IssueId != link.SourceIssueId)
								   .OrderBy(i => i.Key)
								   .ToList();
				ViewBag.AllIssues = new SelectList(allOther, "IssueId", "Key", link.TargetIssueId);
				ViewBag.LinkTypes = new SelectList(new[] { "Blocks", "Relates", "Duplicate" }, link.LinkType);
				return View(link);
			}

			return RedirectToAction("Index", new { issueId = link.SourceIssueId });
		}


		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			var existing = await _service.GetByIdAsync(id);
			if (existing == null) return HttpNotFound();

			int sourceId = existing.SourceIssueId;
			await _service.DeleteAsync(id);
			return RedirectToAction("Index", new { issueId = sourceId });
		}

	}
}
