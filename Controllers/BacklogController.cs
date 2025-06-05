using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sprintify.Context;
using Sprintify.Services;

namespace Sprintify.Controllers
{
	public class BacklogController : Controller
	{
		private readonly BacklogService _service;
		private readonly AppDbContext dbcontext;
		public BacklogController()
		{
			_service = new BacklogService();
			dbcontext = new AppDbContext();
		}
		public async Task<ActionResult> Index(int projectId)
		{
			var backlogIssues = await _service.GetBacklogIssuesAsync(projectId);

			var sprints = dbcontext.Sprints
							  .Where(s => s.ProjectId == projectId)
							  .OrderBy(s => s.StartDate ?? System.DateTime.MinValue)
							  .ToList();
			ViewBag.Sprints = new SelectList(sprints, "SprintId", "Name");

			ViewBag.ProjectId = projectId;
			return View(backlogIssues); 
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> AddToSprint(int projectId, int sprintId, int issueId, int rank = 0)
		{
			var success = await _service.AddIssueToSprintAsync(sprintId, issueId, rank);
			if (!success)
			{
				ModelState.AddModelError("", "Failed to assign issue to sprint (maybe it’s already assigned).");
			}
			return RedirectToAction("Index", new { projectId = projectId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RemoveFromSprint(int projectId, int sprintId, int issueId)
		{
			await _service.RemoveIssueFromSprintAsync(sprintId, issueId);
			return RedirectToAction("Index", new { projectId = projectId });
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				dbcontext.Dispose();
			base.Dispose(disposing);
		}
	}
}
