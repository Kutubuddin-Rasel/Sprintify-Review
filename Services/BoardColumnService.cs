using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Sprintify.Context;
using Sprintify.Models;

namespace Sprintify.Services
{
	public class BoardColumnService
	{
		public async Task<List<BoardColumn>> GetAllByProjectAsync(int projectId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.BoardColumns
								.Include(bc => bc.Project)
								.Where(bc => bc.ProjectId == projectId)
								.OrderBy(bc => bc.ColumnId)
								.ToListAsync();
			}
		}

		public async Task<BoardColumn> GetByIdAsync(int columnId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.BoardColumns
								.Include(bc => bc.Project)
								.FirstOrDefaultAsync(bc => bc.ColumnId == columnId);
			}
		}

		public async Task<int> CreateAsync(BoardColumn column)
		{
			if (column == null) throw new ArgumentNullException(nameof(column));

			using (var dbcontext = new AppDbContext())
			{
				dbcontext.BoardColumns.Add(column);
				await dbcontext.SaveChangesAsync();
				return column.ColumnId;
			}
		}
		public async Task<bool> UpdateAsync(BoardColumn column)
		{
			if (column == null) throw new ArgumentNullException(nameof(column));

			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.BoardColumns.FindAsync(column.ColumnId);
				if (existing == null) return false;

				existing.ProjectId = column.ProjectId;
				existing.Name = column.Name;
				existing.Status = column.Status;
				existing.WIPLimit = column.WIPLimit;

				await dbcontext.SaveChangesAsync();
				return true;
			}
		}

		public async Task<bool> DeleteAsync(int columnId)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.BoardColumns.FindAsync(columnId);
				if (existing == null) return false;

				dbcontext.BoardColumns.Remove(existing);
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}
	}
}
