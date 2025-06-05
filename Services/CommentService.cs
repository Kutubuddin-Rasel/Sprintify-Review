using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Sprintify.Context;
using Sprintify.Models;

namespace Sprintify.Services
{
	public class CommentService
	{
		public async Task<List<Comment>> GetAllByIssueAsync(int issueId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.Comments
								.Include(c => c.Author)
								.Where(c => c.IssueId == issueId)
								.OrderBy(c => c.CreatedAt)
								.ToListAsync();
			}
		}

		public async Task<Comment> GetByIdAsync(int commentId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.Comments
								.Include(c => c.Author)
								.Include(c => c.Issue)
								.FirstOrDefaultAsync(c => c.CommentId == commentId);
			}
		}

		public async Task<int> CreateAsync(Comment comment)
		{
			if (comment == null) throw new ArgumentNullException(nameof(comment));

			comment.CreatedAt = DateTime.UtcNow;

			using (var dbcontext = new AppDbContext())
			{
				dbcontext.Comments.Add(comment);
				await dbcontext.SaveChangesAsync();
				return comment.CommentId;
			}
		}

		public async Task<bool> UpdateAsync(Comment comment)
		{
			if (comment == null) throw new ArgumentNullException(nameof(comment));

			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.Comments.FindAsync(comment.CommentId);
				if (existing == null) return false;

				existing.Body = comment.Body;
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}

		public async Task<bool> DeleteAsync(int commentId)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.Comments.FindAsync(commentId);
				if (existing == null) return false;

				dbcontext.Comments.Remove(existing);
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}
	}
}
