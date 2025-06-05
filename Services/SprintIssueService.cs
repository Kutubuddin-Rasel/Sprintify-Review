using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Sprintify.Context;
using Sprintify.Models;

namespace Sprintify.Services
{
	public class SprintIssueService
	{
		public async Task<List<SprintIssue>> GetAllBySprintAsync(int sprintId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.SprintIssues
								.Include(si => si.Issue)
								.Where(si => si.SprintId == sprintId)
								.OrderBy(si => si.Rank)
								.ThenBy(si => si.IssueId)
								.ToListAsync();
			}
		}

		public async Task<List<SprintIssue>> GetAllByIssueAsync(int issueId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.SprintIssues
								.Include(si => si.Sprint)
								.Where(si => si.IssueId == issueId)
								.ToListAsync();
			}
		}

		public async Task<SprintIssue> GetByIdAsync(int sprintId, int issueId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.SprintIssues
								.Include(si => si.Sprint)
								.Include(si => si.Issue)
								.FirstOrDefaultAsync(si =>
									si.SprintId == sprintId &&
									si.IssueId == issueId);
			}
		}

		public async Task CreateAsync(SprintIssue sprintIssue)
		{
			if (sprintIssue == null) throw new ArgumentNullException(nameof(sprintIssue));

			using (var dbcontext = new AppDbContext())
			{
				bool exists = await dbcontext.SprintIssues.AnyAsync(si =>
					si.SprintId == sprintIssue.SprintId &&
					si.IssueId == sprintIssue.IssueId);
				if (exists)
				{
					throw new InvalidOperationException("This issue is already in the sprint.");
				}

				var maxRank = await dbcontext.SprintIssues
					.Where(si => si.SprintId == sprintIssue.SprintId)
					.MaxAsync(si => (int?)si.Rank) ?? 0;
				sprintIssue.Rank = maxRank + 1;

				dbcontext.SprintIssues.Add(sprintIssue);
				await dbcontext.SaveChangesAsync();
			}
		}

		public async Task<bool> UpdateRankAsync(int sprintId, int issueId, int newRank)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.SprintIssues.FindAsync(sprintId, issueId);
				if (existing == null) return false;

				existing.Rank = newRank;
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}

		public async Task<bool> DeleteAsync(int sprintId, int issueId)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.SprintIssues.FindAsync(sprintId, issueId);
				if (existing == null) return false;

				dbcontext.SprintIssues.Remove(existing);
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}
	}
}
