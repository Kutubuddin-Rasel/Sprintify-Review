using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Sprintify.Context;
using Sprintify.Models;

namespace Sprintify.Services
{
	public class BoardService
	{
		public async Task<List<BoardColumn>> GetColumnsByProjectAsync(int projectId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.BoardColumns
					.Where(bc => bc.ProjectId == projectId)
					.OrderBy(bc => bc.ColumnId)
					.ToListAsync();
			}
		}

		public async Task<List<Issue>> GetIssuesByProjectAsync(int projectId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.Issues
					.Include(i => i.Assignee)
					.Include(i => i.Reporter)
					.Where(i => i.ProjectId == projectId)
					.ToListAsync();
			}
		}
	}
}
