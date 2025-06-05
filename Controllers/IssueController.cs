using Sprintify.Context;
using Sprintify.Models;
using Sprintify.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Sprintify.Controllers
{
	public class IssueController : Controller
	{
		private readonly IssueService _service;
		private readonly AppDbContext dbcontext;

		public IssueController()
		{
			_service = new IssueService();
			dbcontext = new AppDbContext();
		}

		// GET: /Issue?projectId={projectId}
		public async Task<ActionResult> Index(int? projectId = null)
		{
			var list = await _service.GetAllAsync(projectId);
			ViewBag.FilterProjectId = projectId;
			return View(list);
		}

		// GET: /Issue/Details/{id}
		public async Task<ActionResult> Details(int id)
		{
			var issue = await _service.GetByIdAsync(id);
			if (issue == null) return HttpNotFound();
			return View(issue);
		}

		// GET: /Issue/Form?projectId={projectId}      (create new)
		// GET: /Issue/Form/{id}?projectId={projectId} (edit existing)
		public async Task<ActionResult> Form(int? id, int? projectId = null)
		{
			// 1) Projects dropdown
			ViewBag.Projects = new SelectList(
				dbcontext.Projects.OrderBy(p => p.Key),
				"ProjectId",
				"Key",
				projectId ?? 0
			);

			// 2) Users dropdown for both Assignee & Reporter
			// We will create TWO separate SelectLists so we can pre-select each appropriately:
			var usersList = dbcontext.Users.OrderBy(u => u.UserName).ToList();
			ViewBag.Assignees = new SelectList(
				usersList,
				"UserId",
				"UserName"
			);
			ViewBag.Reporters = new SelectList(
				usersList,
				"UserId",
				"UserName"
			);

			// 3) Static lists for Status / IssueType / Priority
			ViewBag.Statuses = new SelectList(new[] { "To Do", "In Progress", "Done" });
			ViewBag.IssueTypes = new SelectList(new[] { "Story", "Bug", "Task" });
			ViewBag.Priorities = new SelectList(new[] { "High", "Medium", "Low" });

			if (id == null || id == 0)
			{
				// Creating a new issue
				var newIssue = new Issue();
				if (projectId.HasValue)
					newIssue.ProjectId = projectId.Value;
				return View(newIssue);
			}
			else
			{
				// Editing an existing issue
				var existing = await _service.GetByIdAsync(id.Value);
				if (existing == null)
					return HttpNotFound();

				// Re‐populate Projects dropdown with selected value
				ViewBag.Projects = new SelectList(
					dbcontext.Projects.OrderBy(p => p.Key),
					"ProjectId",
					"Key",
					existing.ProjectId
				);

				// Re‐populate Assignees dropdown (pre‐select existing.AssigneeId)
				ViewBag.Assignees = new SelectList(
					usersList,
					"UserId",
					"UserName",
					existing.AssigneeId
				);

				// Re‐populate Reporters dropdown (pre‐select existing.ReporterId)
				ViewBag.Reporters = new SelectList(
					usersList,
					"UserId",
					"UserName",
					existing.ReporterId
				);

				// Pre‐select the existing values for Status, IssueType, Priority
				ViewBag.Statuses = new SelectList(
					new[] { "To Do", "In Progress", "Done" },
					existing.Status
				);
				ViewBag.IssueTypes = new SelectList(
					new[] { "Story", "Bug", "Task" },
					existing.IssueType
				);
				ViewBag.Priorities = new SelectList(
					new[] { "High", "Medium", "Low" },
					existing.Priority
				);

				return View(existing);
			}
		}

		// POST: /Issue/Form
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Form(
			[Bind(Include = "IssueId,ProjectId,IssueType,Key,Summary,Description,Priority,Status,AssigneeId,ReporterId")]
			Issue issue
		)
		{
			if (!ModelState.IsValid)
			{
				// If validation fails, re‐populate the same dropdowns with the user’s selected values:
				ViewBag.Projects = new SelectList(
					dbcontext.Projects.OrderBy(p => p.Key),
					"ProjectId",
					"Key",
					issue.ProjectId
				);

				var usersList = dbcontext.Users.OrderBy(u => u.UserName).ToList();
				ViewBag.Assignees = new SelectList(
					usersList,
					"UserId",
					"UserName",
					issue.AssigneeId
				);
				ViewBag.Reporters = new SelectList(
					usersList,
					"UserId",
					"UserName",
					issue.ReporterId
				);

				ViewBag.Statuses = new SelectList(
					new[] { "To Do", "In Progress", "Done" },
					issue.Status
				);
				ViewBag.IssueTypes = new SelectList(
					new[] { "Story", "Bug", "Task" },
					issue.IssueType
				);
				ViewBag.Priorities = new SelectList(
					new[] { "High", "Medium", "Low" },
					issue.Priority
				);

				return View(issue);
			}

			if (issue.IssueId == 0)
			{
				// Create new issue
				var newId = await _service.CreateAsync(issue);
				return RedirectToAction("Details", new { id = newId });
			}
			else
			{
				// Update existing issue
				var success = await _service.UpdateAsync(issue);
				if (!success) return HttpNotFound();
				return RedirectToAction("Index", new { projectId = issue.ProjectId });
			}
		}

		// POST: /Issue/Delete/{id}
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			var issue = await _service.GetByIdAsync(id);
			if (issue == null) return HttpNotFound();

			int projectId = issue.ProjectId;
			await _service.DeleteAsync(id);
			return RedirectToAction("Index", new { projectId });
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				dbcontext.Dispose();
			base.Dispose(disposing);
		}
	}
}
