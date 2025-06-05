using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Sprintify.Context;
using Sprintify.Models;

namespace Sprintify.Services
{
	public class IssueHistoryService
	{
		public async Task<List<IssueHistory>> GetAllByIssueAsync(int issueId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.IssueHistories
								.Include(h => h.Issue)
								.Include(h => h.ChangedBy)
								.Where(h => h.IssueId == issueId)
								.OrderBy(h => h.ChangedAt)
								.ToListAsync();
			}
		}
		public async Task<IssueHistory> GetByIdAsync(int historyId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.IssueHistories
								.Include(h => h.Issue)
								.Include(h => h.ChangedBy)
								.FirstOrDefaultAsync(h => h.HistoryId == historyId);
			}
		}

		public async Task<int> CreateAsync(IssueHistory history)
		{
			if (history == null) throw new ArgumentNullException(nameof(history));

			history.ChangedAt = DateTime.UtcNow;
			using (var dbcontext = new AppDbContext())
			{
				dbcontext.IssueHistories.Add(history);
				await dbcontext.SaveChangesAsync();
				return history.HistoryId;
			}
		}

		public async Task<bool> UpdateAsync(IssueHistory history)
		{
			if (history == null) throw new ArgumentNullException(nameof(history));

			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.IssueHistories.FindAsync(history.HistoryId);
				if (existing == null) return false;

				existing.FieldName = history.FieldName;
				existing.OldValue = history.OldValue;
				existing.NewValue = history.NewValue;
				existing.ChangedById = history.ChangedById;

				await dbcontext.SaveChangesAsync();
				return true;
			}
		}

		public async Task<bool> DeleteAsync(int historyId)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.IssueHistories.FindAsync(historyId);
				if (existing == null) return false;

				dbcontext.IssueHistories.Remove(existing);
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}
	}
}
