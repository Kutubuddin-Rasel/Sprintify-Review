using System.Threading.Tasks;
using System.Web.Mvc;
using Sprintify.Context;
using Sprintify.Models;
using Sprintify.Services;

namespace Sprintify.Controllers
{
	public class BoardColumnController : Controller
	{
		private readonly BoardColumnService _service ;
		private readonly AppDbContext dbcontext;
		public BoardColumnController()
		{
			_service = new BoardColumnService();
			dbcontext = new AppDbContext();
		}
		public async Task<ActionResult> Index(int projectId)
		{
			var list = await _service.GetAllByProjectAsync(projectId);
			ViewBag.ProjectId = projectId;
			return View(list);  
		}

		public async Task<ActionResult> Details(int id)
		{
			var column = await _service.GetByIdAsync(id);
			if (column == null) return HttpNotFound();
			return View(column); 
		}

		public async Task<ActionResult> Form(int? id, int? projectId = null)
		{
			ViewBag.Projects = new SelectList(dbcontext.Projects, "ProjectId", "Key", projectId ?? 0);

			if (id == null || id == 0)
			{
				var newColumn = new BoardColumn();
				if (projectId.HasValue)
					newColumn.ProjectId = projectId.Value;
				return View(newColumn);
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
			[Bind(Include = "ColumnId,ProjectId,Name,Status,WIPLimit")]
			BoardColumn column
		)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Projects = new SelectList(
					dbcontext.Projects, "ProjectId", "Key", column.ProjectId
				);
				return View(column);
			}

			if (column.ColumnId == 0)
			{
				var newId = await _service.CreateAsync(column);
				return RedirectToAction("Details", new { id = newId });
			}
			else
			{
				var success = await _service.UpdateAsync(column);
				if (!success) return HttpNotFound();

				return RedirectToAction("Index", new { projectId = column.ProjectId });
			}
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			var column = await _service.GetByIdAsync(id);
			if (column == null) return HttpNotFound();

			int projectId = column.ProjectId;
			await _service.DeleteAsync(id);
			return RedirectToAction("Index", new { projectId });
		}
	}
}
