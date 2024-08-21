using BlazorAuthTemplate.Data;
using BlazorAuthTemplate.Models;
using BlazorAuthTemplate.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthTemplate.Services
{
	public class CommentRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : ICommentRepository
	{
		public async Task<Comment> CreateCommentAsync(Comment comment)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			context.Comments.Add(comment);
			await context.SaveChangesAsync();

			return comment;
		}

		public async Task DeleteCommentAsync(int commentId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			Comment? comment = await context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

			if (comment != null)
			{
				context.Comments.Remove(comment);
				await context.SaveChangesAsync();
			}
		}

		public async Task<Comment?> GetCommentByIdAsync(int commentId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			Comment? comment = await context.Comments
				.FirstOrDefaultAsync(c => c.Id == commentId);

			return comment;
		}

		public async Task<IEnumerable<Comment>> GetCommentsAsync(int postId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			List<Comment> comments = await context.Comments
				.Where(c => c.BlogPostId == postId)
				.ToListAsync();

			return comments;
		}

		public async Task UpdateCommentAsync(Comment comment)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			if (await context.Comments.AnyAsync(c => c.Id == comment.Id))
			{
				context.Comments.Update(comment);
				await context.SaveChangesAsync();
			}
		}
	}
}
