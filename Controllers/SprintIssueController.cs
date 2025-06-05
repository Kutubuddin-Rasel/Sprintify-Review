using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sprintify.Context;
using Sprintify.Models;
using Sprintify.Services;

namespace Sprintify.Controllers
{
	public class SprintIssueController : Controller
	{
		private readonly SprintIssueService _service;
		private readonly AppDbContext dbcontext;
		public SprintIssueController()
		{
			_service = new SprintIssueService();
			dbcontext = new AppDbContext();
		}


		public async Task<ActionResult> Index(int sprintId)
		{
			var sprint = await new SprintService().GetByIdAsync(sprintId);
			if (sprint == null) return HttpNotFound();

			ViewBag.SprintId = sprintId;
			ViewBag.SprintName = sprint.Name;
			ViewBag.ProjectId = sprint.ProjectId;
			ViewBag.ProjectKey = sprint.Project.Key;

			var list = await _service.GetAllBySprintAsync(sprintId);
			return View(list);  
		}


		public async Task<ActionResult> Details(int sprintId, int issueId)
		{
			var rec = await _service.GetByIdAsync(sprintId, issueId);
			if (rec == null) return HttpNotFound();
			return View(rec);
		}


		public async Task<ActionResult> Form(int sprintId, int? issueId = null)
		{
			var sprint = await new SprintService().GetByIdAsync(sprintId);
			if (sprint == null) return HttpNotFound();

			ViewBag.SprintId = sprintId;
			ViewBag.SprintName = sprint.Name;
			ViewBag.ProjectId = sprint.ProjectId;
			ViewBag.ProjectKey = sprint.Project.Key;

			var assignedIssueIds = dbcontext.SprintIssues
									   .Where(si => si.SprintId == sprintId)
									   .Select(si => si.IssueId)
									   .ToList();

			var openIssues = dbcontext.Issues
								 .Where(i => i.ProjectId == sprint.ProjectId
										  && !assignedIssueIds.Contains(i.IssueId))
								 .OrderBy(i => i.Key)
								 .ToList();

			ViewBag.AllIssues = new SelectList(openIssues, "IssueId", "Key");

			if (issueId == null)
			{
				var newSi = new SprintIssue { SprintId = sprintId };
				return View(newSi);
			}
			else
			{
				var existing = await _service.GetByIdAsync(sprintId, issueId.Value);
				if (existing == null) return HttpNotFound();

				return View(existing);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Form(
			[Bind(Include = "SprintId,IssueId,Rank")]
			SprintIssue sprintIssue,
			int? originalIssueId = null
		)
		{
			var sprint = await new SprintService().GetByIdAsync(sprintIssue.SprintId);
			if (sprint == null) return HttpNotFound();

			ViewBag.SprintId = sprintIssue.SprintId;
			ViewBag.SprintName = sprint.Name;
			ViewBag.ProjectId = sprint.ProjectId;
			ViewBag.ProjectKey = sprint.Project.Key;

			var assignedIds = dbcontext.SprintIssues
								  .Where(si => si.SprintId == sprintIssue.SprintId)
								  .Select(si => si.IssueId)
								  .ToList();
			var openIssues = dbcontext.Issues
								 .Where(i => i.ProjectId == sprint.ProjectId
										  && !assignedIds.Contains(i.IssueId))
								 .OrderBy(i => i.Key)
								 .ToList();
			ViewBag.AllIssues = new SelectList(openIssues, "IssueId", "Key");

			if (!ModelState.IsValid)
			{
				return View(sprintIssue);
			}

			if (originalIssueId == null)
			{
				// CREATE new assignment
				try
				{
					await _service.CreateAsync(sprintIssue);
				}
				catch (InvalidOperationException ex)
				{
					ModelState.AddModelError("", ex.Message);
					return View(sprintIssue);
				}
			}
			else
			{
				var success = await _service.UpdateRankAsync(
					sprintIssue.SprintId,
					sprintIssue.IssueId,
					sprintIssue.Rank
				);
				if (!success) return HttpNotFound();
			}

			return RedirectToAction("Index", new { sprintId = sprintIssue.SprintId });
		}


		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int sprintId, int issueId)
		{
			var existing = await _service.GetByIdAsync(sprintId, issueId);
			if (existing == null) return HttpNotFound();

			await _service.DeleteAsync(sprintId, issueId);
			return RedirectToAction("Index", new { sprintId = sprintId });
		}

	}
}
