using System.Threading.Tasks;
using System.Web.Mvc;
using Sprintify.Context;
using Sprintify.Models;
using Sprintify.Services;

namespace Sprintify.Controllers
{
	public class CommentController : Controller
	{
		private readonly CommentService _service;
		private readonly AppDbContext dbcontext;
		public CommentController()
		{
			dbcontext = new AppDbContext();
			_service = new CommentService();
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

		public async Task<ActionResult> Form(int? id, int issueId)
		{
			var issue = await new IssueService().GetByIdAsync(issueId);
			if (issue == null) return HttpNotFound();

			ViewBag.IssueId = issueId;
			ViewBag.IssueKey = issue.Key;

			ViewBag.Users = new SelectList(dbcontext.Users, "UserId", "UserId");

			if (id == null || id == 0)
			{
				var newComment = new Comment
				{
					IssueId = issueId
				};
				return View(newComment);
			}
			else
			{
				var existing = await _service.GetByIdAsync(id.Value);
				if (existing == null) return HttpNotFound();

				ViewBag.Users = new SelectList(
					dbcontext.Users, "UserId", "UserId", existing.UserId
				);
				return View(existing);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Form(
			[Bind(Include = "CommentId,IssueId,UserId,Body")]
			Comment comment
		)
		{
			var issue = await new IssueService().GetByIdAsync(comment.IssueId);
			if (issue == null) return HttpNotFound();

			if (!ModelState.IsValid)
			{
				ViewBag.IssueId = comment.IssueId;
				ViewBag.IssueKey = issue.Key;
				ViewBag.Users = new SelectList(dbcontext.Users, "UserId", "UserId", comment.UserId);
				return View(comment);
			}

			if (comment.CommentId == 0)
			{
				var newId = await _service.CreateAsync(comment);
				return RedirectToAction("Index", new { issueId = comment.IssueId });
			}
			else
			{
				var success = await _service.UpdateAsync(comment);
				if (!success) return HttpNotFound();
				return RedirectToAction("Index", new { issueId = comment.IssueId });
			}
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
