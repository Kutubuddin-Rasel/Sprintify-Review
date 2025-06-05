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
	public class SprintService
	{
		public async Task<List<Sprint>> GetAllAsync(int? projectId = null)
		{
			using (var dbcontext = new AppDbContext())
			{
				IQueryable<Sprint> query = dbcontext.Sprints
					.Include(s => s.Project)       
					.Include(s => s.SprintIssues.Select(si => si.Issue)); 

				if (projectId.HasValue)
					query = query.Where(s => s.ProjectId == projectId.Value);

				return await query.ToListAsync();
			}
		}

		public async Task<Sprint> GetByIdAsync(int sprintId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.Sprints
					.Include(s => s.Project)
					.Include(s => s.SprintIssues.Select(si => si.Issue))
					.FirstOrDefaultAsync(s => s.SprintId == sprintId);
			}
		}

		public async Task<int> CreateAsync(Sprint sprint)
		{
			if (sprint == null) throw new ArgumentNullException(nameof(sprint));

			sprint.Status = string.IsNullOrWhiteSpace(sprint.Status) ? "Planned" : sprint.Status;

			using (var dbcontext = new AppDbContext())
			{
				dbcontext.Sprints.Add(sprint);
				await dbcontext.SaveChangesAsync();
				return sprint.SprintId;
			}
		}

		public async Task<bool> UpdateAsync(Sprint sprint)
		{
			if (sprint == null) throw new ArgumentNullException(nameof(sprint));

			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.Sprints.FindAsync(sprint.SprintId);
				if (existing == null) return false;

				existing.Name = sprint.Name;
				existing.ProjectId = sprint.ProjectId;
				existing.StartDate = sprint.StartDate;
				existing.EndDate = sprint.EndDate;
				existing.Goal = sprint.Goal;
				existing.Status = sprint.Status;

				await dbcontext.SaveChangesAsync();
				return true;
			}
		}

		public async Task<bool> DeleteAsync(int sprintId)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.Sprints.FindAsync(sprintId);
				if (existing == null) return false;

				dbcontext.Sprints.Remove(existing);
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}

		public async Task<bool> StartAsync(int sprintId)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.Sprints.FindAsync(sprintId);
				if (existing == null) return false;

				existing.Status = "Active";
				if (!existing.StartDate.HasValue)
					existing.StartDate = DateTime.UtcNow.Date;

				await dbcontext.SaveChangesAsync();
				return true;
			}
		}

		public async Task<bool> CompleteAsync(int sprintId)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.Sprints.FindAsync(sprintId);
				if (existing == null) return false;

				existing.Status = "Completed";
				if (!existing.EndDate.HasValue)
					existing.EndDate = DateTime.UtcNow.Date;

				await dbcontext.SaveChangesAsync();
				return true;
			}
		}
	}
}