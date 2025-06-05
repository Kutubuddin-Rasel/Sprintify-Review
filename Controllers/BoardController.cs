using System.Threading.Tasks;
using System.Web.Mvc;
using Sprintify.Context;
using Sprintify.Services;

namespace Sprintify.Controllers
{
	public class BoardController : Controller
	{
		private readonly AppDbContext dbcontext;
		public BoardController() { 
			dbcontext = new AppDbContext();
		}
		public ActionResult Index(int projectId)
		{
			var project = dbcontext.Projects.Find(projectId);
			if (project == null) return HttpNotFound();

			ViewBag.ProjectId = projectId;
			ViewBag.ProjectKey = project.Key;
			ViewBag.ProjectName = project.Name;

			return View(); 
		}

	}
}
