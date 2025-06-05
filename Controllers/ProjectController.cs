using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sprintify.Context;
using Sprintify.Models;
using Sprintify.Services;

namespace Sprintify.Controllers
{
	public class ProjectController : Controller
	{
		private readonly ProjectService _service;
		private readonly AppDbContext dbcontext;

		public ProjectController()
		{
			_service = new ProjectService();
			dbcontext = new AppDbContext();
		}

		// GET: /Project
		public async Task<ActionResult> Index()
		{
			var list = await _service.GetAllAsync();
			return View(list);
		}

		// GET: /Project/Details/{id}
		public async Task<ActionResult> Details(int id)
		{
			var project = await _service.GetByIdAsync(id);
			if (project == null)
				return HttpNotFound();
			return View(project);
		}

		// GET: /Project/Form        (new)
		// GET: /Project/Form/{id}   (edit)
		public async Task<ActionResult> Form(int? id)
		{
			// Populate the “LeadUsers” dropdown as [UserId, UserName], not [UserId, UserId]
			ViewBag.LeadUsers = new SelectList(
				dbcontext.Users.OrderBy(u => u.UserName),
				"UserId",
				"UserName"
			);

			if (id == null || id == 0)
			{
				// Creating a blank form for a new Project
				return View(new Project());
			}
			else
			{
				// Editing an existing project
				var existing = await _service.GetByIdAsync(id.Value);
				if (existing == null)
					return HttpNotFound();

				// Pre-select the current LeadUserId in the dropdown
				ViewBag.LeadUsers = new SelectList(
					dbcontext.Users.OrderBy(u => u.UserName),
					"UserId",
					"UserName",
					existing.LeadUserId
				);
				return View(existing);
			}
		}

		// POST: /Project/Form
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Form(
			[Bind(Include = "ProjectId,Key,Name,LeadUserId")]
			Project project
		)
		{
			if (!ModelState.IsValid)
			{
				// If validation fails, re-populate the dropdown so the form can redisplay correctly
				ViewBag.LeadUsers = new SelectList(
					dbcontext.Users.OrderBy(u => u.UserName),
					"UserId",
					"UserName",
					project.LeadUserId
				);
				return View(project);
			}

			if (project.ProjectId == 0)
			{
				// Create new
				project.CreatedAt = DateTime.UtcNow;
				var newId = await _service.CreateAsync(project);
				return RedirectToAction("Details", new { id = newId });
			}
			else
			{
				// Update existing
				var success = await _service.UpdateAsync(project);
				if (!success)
					return HttpNotFound();
				return RedirectToAction("Index");
			}
		}

		// POST: /Project/Delete/{id}
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			await _service.DeleteAsync(id);
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				dbcontext.Dispose();
			base.Dispose(disposing);
		}
	}
}
