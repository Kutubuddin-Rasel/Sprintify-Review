using Sprintify.Context;
using Sprintify.Models;
using Sprintify.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Sprintify.Controllers
{
    public class SprintController : Controller
	{
		private readonly SprintService _service;
		private readonly AppDbContext dbcontext;
		public SprintController()
		{
			_service = new SprintService();
			dbcontext = new AppDbContext();
		}

		public async Task<ActionResult> Index(int? projectId = null)
		{
			var list = await _service.GetAllAsync(projectId);
			ViewBag.FilterProjectId = projectId;
			return View(list);
		}

		public async Task<ActionResult> Details(int id)
		{
			var sprint = await _service.GetByIdAsync(id);
			if (sprint == null) return HttpNotFound();
			return View(sprint); 
		}

		public async Task<ActionResult> Form(int? id, int? projectId = null)
		{
			ViewBag.Projects = new SelectList(dbcontext.Projects, "ProjectId", "Key", projectId ?? 0);

			if (id == null || id == 0)
			{
				var newSprint = new Sprint();
				if (projectId.HasValue) newSprint.ProjectId = projectId.Value;
				newSprint.Status = "Planned";
				return View(newSprint);
			}
			else
			{
				var existing = await _service.GetByIdAsync(id.Value);
				if (existing == null) return HttpNotFound();

				ViewBag.Projects = new SelectList(
					dbcontext.Projects, "ProjectId", "Key", existing.ProjectId
				);
				return View(existing);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Form(
			[Bind(Include = "SprintId,ProjectId,Name,StartDate,EndDate,Goal,Status")]
			Sprint sprint
		)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Projects = new SelectList(dbcontext.Projects, "ProjectId", "Key", sprint.ProjectId);
				return View(sprint);
			}

			if (sprint.SprintId == 0)
			{
				var newId = await _service.CreateAsync(sprint);
				return RedirectToAction("Details", new { id = newId });
			}
			else
			{
				var success = await _service.UpdateAsync(sprint);
				if (!success) return HttpNotFound();
				return RedirectToAction("Index", new { projectId = sprint.ProjectId });
			}
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			var sprint = await _service.GetByIdAsync(id);
			if (sprint == null) return HttpNotFound();

			int projectId = sprint.ProjectId;
			await _service.DeleteAsync(id);
			return RedirectToAction("Index", new { projectId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Start(int id)
		{
			var sprint = await _service.GetByIdAsync(id);
			if (sprint == null) return HttpNotFound();

			await _service.StartAsync(id);
			return RedirectToAction("Details", new { id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Complete(int id)
		{
			var sprint = await _service.GetByIdAsync(id);
			if (sprint == null) return HttpNotFound();

			await _service.CompleteAsync(id);
			return RedirectToAction("Details", new { id });
		}

	}
}