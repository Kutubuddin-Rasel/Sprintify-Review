using Sprintify.Context;
using Sprintify.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Sprintify.Services
{
	public class IssueService
	{
		public async Task<List<Issue>> GetAllAsync(int? projectId = null)
		{
			using (var dbcontext = new AppDbContext())
			{
				IQueryable<Issue> query = dbcontext.Issues
					.Include(i => i.Project)
					.Include(i => i.Assignee)
					.Include(i => i.Reporter);

				if (projectId.HasValue)
				{
					query = query.Where(i => i.ProjectId == projectId.Value);
				}

				return await query.ToListAsync();
			}
		}

		public async Task<Issue> GetByIdAsync(int issueId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.Issues
					.Include(i => i.Project)
					.Include(i => i.Assignee)
					.Include(i => i.Reporter)
					.FirstOrDefaultAsync(i => i.IssueId == issueId);
			}
		}

		public async Task<int> CreateAsync(Issue issue)
		{
			if (issue == null) throw new ArgumentNullException(nameof(issue));

			issue.CreatedAt = DateTime.UtcNow;
			issue.UpdatedAt = DateTime.UtcNow;

			using (var dbcontext = new AppDbContext())
			{
				dbcontext.Issues.Add(issue);
				await dbcontext.SaveChangesAsync();
				return issue.IssueId;
			}
		}

		public async Task<bool> UpdateAsync(Issue issue)
		{
			if (issue == null) throw new ArgumentNullException(nameof(issue));

			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.Issues.FindAsync(issue.IssueId);
				if (existing == null) return false;

				existing.IssueType = issue.IssueType;
				existing.Key = issue.Key;
				existing.Summary = issue.Summary;
				existing.Description = issue.Description;
				existing.Priority = issue.Priority;
				existing.Status = issue.Status;
				existing.AssigneeId = issue.AssigneeId;
				existing.ReporterId = issue.ReporterId;
				existing.UpdatedAt = DateTime.UtcNow;

				await dbcontext.SaveChangesAsync();
				return true;
			}
		}

		public async Task<bool> DeleteAsync(int issueId)
		{
			using (var ctx = new AppDbContext())
			{
				var existing = await ctx.Issues.FindAsync(issueId);
				if (existing == null) return false;

				ctx.Issues.Remove(existing);
				await ctx.SaveChangesAsync();
				return true;
			}
		}
	}
}