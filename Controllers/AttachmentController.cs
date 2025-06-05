using System;
using System.Data.Entity;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Sprintify.Context;
using Sprintify.Models;
using Sprintify.Services;

namespace Sprintify.Controllers
{
	public class AttachmentController : Controller
	{
		private readonly AttachmentService _service ;
		private readonly AppDbContext dbcontext;
		public AttachmentController()
		{
			dbcontext = new AppDbContext();
			_service = new AttachmentService();
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
			var attachment = await _service.GetByIdAsync(id);
			if (attachment == null) return HttpNotFound();
			return View(attachment); 
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
				var newAttach = new Attachment { IssueId = issueId };
				return View(newAttach);
			}
			else
			{
				var existing = await _service.GetByIdAsync(id.Value);
				if (existing == null) return HttpNotFound();

				ViewBag.Users = new SelectList(dbcontext.Users, "UserId", "UserId", existing.UploadedById);
				return View(existing);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Form(
			int issueId,
			int? attachmentId,
			int uploadedById,
			HttpPostedFileBase file 
		)
		{
			var issue = await new IssueService().GetByIdAsync(issueId);
			if (issue == null) return HttpNotFound();

			ViewBag.IssueId = issueId;
			ViewBag.IssueKey = issue.Key;
			ViewBag.Users = new SelectList(dbcontext.Users, "UserId", "UserId", uploadedById);

			if (attachmentId == null || attachmentId == 0)
			{
				if (file == null || file.ContentLength == 0)
				{
					ModelState.AddModelError("file", "Please select a file to upload.");
					return View(new Attachment { IssueId = issueId, UploadedById = uploadedById });
				}

				string uploadsFolder = Server.MapPath("~/Uploads");
				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
				string safeName = Path.GetFileName(file.FileName);
				string localName = $"issue_{issueId}{timestamp}{safeName}";
				string fullPath = Path.Combine(uploadsFolder, localName);
				file.SaveAs(fullPath);

				var attachment = new Attachment
				{
					IssueId = issueId,
					FileName = safeName,
					FilePath = $"~/Uploads/{localName}",
					UploadedById = uploadedById
				};
				await _service.CreateAsync(attachment);

				return RedirectToAction("Index", new { issueId = issueId });
			}
			else
			{
				var existing = await _service.GetByIdAsync(attachmentId.Value);
				if (existing == null) return HttpNotFound();

				existing.UploadedById = uploadedById;
				using (var ctx = new AppDbContext())
				{
					ctx.Entry(existing).State = EntityState.Modified;
					await ctx.SaveChangesAsync();
				}

				return RedirectToAction("Index", new { issueId = issueId });
			}
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			var existing = await _service.GetByIdAsync(id);
			if (existing == null) return HttpNotFound();

			int issueId = existing.IssueId;

			string appRoot = Server.MapPath("~");
			string relative = existing.FilePath.TrimStart('~', '/')
								 .Replace('/', Path.DirectorySeparatorChar);
			string fullPath = Path.Combine(appRoot, relative);
			if (System.IO.File.Exists(fullPath))
			{
				System.IO.File.Delete(fullPath);
			}

			await _service.DeleteAsync(id);

			return RedirectToAction("Index", new { issueId });
		}

	}
}