using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Sprintify.Context;
using Sprintify.Models;

namespace Sprintify.Services
{
	public class AttachmentService
	{
		public async Task<List<Attachment>> GetAllByIssueAsync(int issueId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.Attachments
								.Include(a => a.Uploader)
								.Where(a => a.IssueId == issueId)
								.OrderByDescending(a => a.UploadedAt)
								.ToListAsync();
			}
		}

		public async Task<Attachment> GetByIdAsync(int attachmentId)
		{
			using (var dbcontext = new AppDbContext())
			{
				return await dbcontext.Attachments
								.Include(a => a.Uploader)
								.Include(a => a.Issue)
								.FirstOrDefaultAsync(a => a.AttachmentId == attachmentId);
			}
		}

		public async Task<int> CreateAsync(Attachment attachment)
		{
			if (attachment == null) throw new ArgumentNullException(nameof(attachment));

			attachment.UploadedAt = DateTime.UtcNow;

			using (var dbcontext = new AppDbContext())
			{
				dbcontext.Attachments.Add(attachment);
				await dbcontext.SaveChangesAsync();
				return attachment.AttachmentId;
			}
		}

		public async Task<bool> DeleteAsync(int attachmentId)
		{
			using (var dbcontext = new AppDbContext())
			{
				var existing = await dbcontext.Attachments.FindAsync(attachmentId);
				if (existing == null) return false;

				dbcontext.Attachments.Remove(existing);
				await dbcontext.SaveChangesAsync();
				return true;
			}
		}
	}
}