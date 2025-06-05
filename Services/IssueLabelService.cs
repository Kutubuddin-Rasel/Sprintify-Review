using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Sprintify.Context;
using Sprintify.Models;

namespace Sprintify.Services
{
	public class IssueLabelService
	{
		public async Task<List<IssueLabel>> GetAllByIssueAsync(int issueId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.IssueLabels
					            .Include(il =>il.Issue)
								.Where(il => il.IssueId == issueId)
								.ToListAsync();
			}
		}

		public async Task<IssueLabel> GetByIdAsync(int issueId, string label)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.IssueLabels
					            .Include(il =>il.Issue)
								.FirstOrDefaultAsync(il => il.IssueId == issueId && 
								il.Label == label);
			}
		}

		public async Task CreateAsync(IssueLabel issueLabel)
		{
			if (issueLabel == null) throw new ArgumentNullException(nameof(issueLabel));

			using (var dbcontext = new AppDbContext())
			{
				bool exists = await dbcontext.IssueLabels.AnyAsync(il =>
					il.IssueId == issueLabel.IssueId &&
					il.Label == issueLabel.Label);

				if (exists)
					throw new InvalidOperationException("This label is already assigned to the issue.");

				dbcontext.IssueLabels.Add(issueLabel);
				await dbcontext.SaveChangesAsync();
			}
		}

		public async Task<bool> DeleteAsync(int issueId, string label)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.IssueLabels.FindAsync(issueId, label);
				if (existing == null) return false;

				dbcontext.IssueLabels.Remove(existing);
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}

		public async Task UpdateAsync(int issueId, string oldLabel, string newLabel)
		{
			if (string.IsNullOrWhiteSpace(newLabel))
				throw new ArgumentException("New label cannot be empty.", nameof(newLabel));

			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.IssueLabels.FindAsync(issueId, oldLabel);
				if (existing == null)
					throw new KeyNotFoundException("Original label not found.");

				bool conflict = await dbcontext.IssueLabels.AnyAsync(il =>
					il.IssueId == issueId && il.Label == newLabel);
				if (conflict)
					throw new InvalidOperationException("A label with the new name already exists on this issue.");

				dbcontext.IssueLabels.Remove(existing);

				var updated = new IssueLabel
				{
					IssueId = issueId,
					Label = newLabel
				};
				dbcontext.IssueLabels.Add(updated);

				await dbcontext.SaveChangesAsync();
			}
		}
	}
}
