using Sprintify.Context;
using Sprintify.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
namespace Sprintify.Services
{
	public class BacklogService
	{
		public async Task<List<Issue>> GetBacklogIssuesAsync(int projectId)
		{
			using (var dbcontext = new AppDbContext())
			{
				var query = dbcontext.Issues
					.Include(i => i.Assignee)
					.Include(i => i.Reporter)
					.Where(i => i.ProjectId == projectId);

				var backlog = await query
					.Where(i => !dbcontext.SprintIssues
						.Any(si => si.IssueId == i.IssueId))
					.ToListAsync();

				return backlog;
			}
		}

		public async Task<bool> AddIssueToSprintAsync(int sprintId, int issueId, int rank = 0)
		{
			using (var dbcontext = new AppDbContext())
			{
				var sprint = await dbcontext.Sprints.FindAsync(sprintId);
				if (sprint == null) return false;

				var issue = await dbcontext.Issues.FindAsync(issueId);
				if (issue == null) return false;

				bool alreadyAssigned = await dbcontext.SprintIssues
					.AnyAsync(si => si.SprintId == sprintId && si.IssueId == issueId);
				if (alreadyAssigned) return false;

				var siEntity = new SprintIssue
				{
					SprintId = sprintId,
					IssueId = issueId,
					Rank = rank
				};
				dbcontext.SprintIssues.Add(siEntity);
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}

		public async Task<bool> RemoveIssueFromSprintAsync(int sprintId, int issueId)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.SprintIssues
					.FirstOrDefaultAsync(si => si.SprintId == sprintId && si.IssueId == issueId);
				if (existing == null) return false;

				dbcontext.SprintIssues.Remove(existing);
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}
	}
}