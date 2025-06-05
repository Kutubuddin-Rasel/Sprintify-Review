using System.Threading.Tasks;
using System.Web.Mvc;
using Sprintify.Context;
using Sprintify.Models;
using Sprintify.Services;

namespace Sprintify.Controllers
{
	public class IssueHistoryController : Controller
	{
		private readonly IssueHistoryService _service ;
		private readonly AppDbContext dbcontext;
		public IssueHistoryController()
		{
			_service = new IssueHistoryService();
			dbcontext = new AppDbContext();
		}

		public async Task<ActionResult> Index(int issueId)
		{
			var issue = await new IssueService().GetByIdAsync(issueId);
			if (issue == null) return HttpNotFound();

			ViewBag.IssueId = issueId;
			ViewBag.IssueKey = issue.Key;

			var list = await _service.GetAllByIssueAsync(issueId);
			return View(list); 
		}

		public async Task<ActionResult> Details(int id)
		{
			var history = await _service.GetByIdAsync(id);
			if (history == null) return HttpNotFound();

			return View(history);
		}

		public async Task<ActionResult> Form(int? id, int issueId)
		{
			var issue = await new IssueService().GetByIdAsync(issueId);
			if (issue == null) return HttpNotFound();

			ViewBag.IssueId = issueId;
			ViewBag.IssueKey = issue.Key;

			ViewBag.Users = new SelectList(dbcontext.Users, "UserId", "UserId");

			if (id == null || id == 0)
			{
				var newHistory = new IssueHistory { IssueId = issueId };
				return View(newHistory);
			}
			else
			{
				var existing = await _service.GetByIdAsync(id.Value);
				if (existing == null) return HttpNotFound();

				ViewBag.Users = new SelectList(dbcontext.Users, "UserId", "UserId", existing.ChangedById);
				return View(existing);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Form(
			[Bind(Include = "HistoryId,IssueId,ChangedById,FieldName,OldValue,NewValue")]
			IssueHistory history
		)
		{
			var issue = await new IssueService().GetByIdAsync(history.IssueId);
			if (issue == null) return HttpNotFound();

			if (!ModelState.IsValid)
			{
				ViewBag.IssueId = history.IssueId;
				ViewBag.IssueKey = issue.Key;
				ViewBag.Users = new SelectList(dbcontext.Users, "UserId", "UserId", history.ChangedById);
				return View(history);
			}

			if (history.HistoryId == 0)
			{
				await _service.CreateAsync(history);
			}
			else
			{
				var success = await _service.UpdateAsync(history);
				if (!success) return HttpNotFound();
			}

			return RedirectToAction("Index", new { issueId = history.IssueId });
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			var existing = await _service.GetByIdAsync(id);
			if (existing == null) return HttpNotFound();

			int issueId = existing.IssueId;
			await _service.DeleteAsync(id);
			return RedirectToAction("Index", new { issueId = issueId });
		}

	}
}