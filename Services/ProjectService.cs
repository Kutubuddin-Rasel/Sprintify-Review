using Sprintify.Context;
using Sprintify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;

namespace Sprintify.Services
{
	public class ProjectService
	{
		public async Task<List<Project>> GetAllAsync()
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.Projects.Include(p=>p.Lead).ToListAsync();
			}
		}
		public async Task<Project> GetByIdAsync(int projectId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.Projects.Include(p => p.Lead).FirstOrDefaultAsync(p => p.ProjectId == projectId);
			}
		}
		public async Task<int> CreateAsync(Project project)
		{
			if(project == null) throw new ArgumentNullException (nameof(project));
			project.CreatedAt = DateTime.UtcNow;
			using (var dbcontext = new AppDbContext())
			{
				dbcontext.Projects.Add(project);
				await dbcontext.SaveChangesAsync();
				return project.ProjectId;
			}
		}
		public async Task<bool> UpdateAsync(Project project)
		{
			if (project == null) throw new ArgumentNullException(nameof(project));
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.Projects.FindAsync(project.ProjectId);
				if (existing == null) return false;
				existing.Key = project.Key;
				existing.Name = project.Name;
				existing.LeadUserId = project.LeadUserId;

				await dbcontext.SaveChangesAsync();
				return true;
			}
		}
		public async Task<bool> DeleteAsync(int projectId)
		{
			using(var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.Projects.FindAsync(projectId);
				if (existing == null) return false;
				dbcontext.Projects.Remove(existing);
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}
	}
}