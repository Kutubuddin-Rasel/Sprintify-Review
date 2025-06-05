using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Sprintify.Context;
using Sprintify.Models;

namespace Sprintify.Services
{
	public class IssueLinkService
	{
		public async Task<List<IssueLink>> GetLinksBySourceAsync(int sourceIssueId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.IssueLinks
					.Include(il => il.Target)
					.Where(il => il.SourceIssueId == sourceIssueId)
					.ToListAsync();
			}
		}

		public async Task<List<IssueLink>> GetLinksByTargetAsync(int targetIssueId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.IssueLinks
					.Include(il => il.Source)
					.Where(il => il.TargetIssueId == targetIssueId)
					.ToListAsync();
			}
		}

		public async Task<IssueLink> GetByIdAsync(int issueLinkId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.IssueLinks
					.Include(il => il.Source)
					.Include(il => il.Target)
					.FirstOrDefaultAsync(il => il.IssueLinkId == issueLinkId);
			}
		}

		public async Task<int> CreateAsync(IssueLink link)
		{
			if (link == null) throw new ArgumentNullException(nameof(link));

			if (link.SourceIssueId == link.TargetIssueId)
				throw new InvalidOperationException("Cannot link an issue to itself.");

			using (var dbcontext = new AppDbContext())
			{
				bool exists = await dbcontext.IssueLinks.AnyAsync(il =>
					il.SourceIssueId == link.SourceIssueId &&
					il.TargetIssueId == link.TargetIssueId &&
					il.LinkType == link.LinkType
				);
				if (exists)
					throw new InvalidOperationException("This link already exists.");

				dbcontext.IssueLinks.Add(link);
				await dbcontext.SaveChangesAsync();
				return link.IssueLinkId;
			}
		}

		public async Task<bool> UpdateAsync(IssueLink link)
		{
			if (link == null) throw new ArgumentNullException(nameof(link));

			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.IssueLinks.FindAsync(link.IssueLinkId);
				if (existing == null) return false;

				existing.LinkType = link.LinkType;
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}

		public async Task<bool> DeleteAsync(int issueLinkId)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.IssueLinks.FindAsync(issueLinkId);
				if (existing == null) return false;

				dbcontext.IssueLinks.Remove(existing);
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}
	}
}
