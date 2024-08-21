using BlazorAuthTemplate.Data;
using BlazorAuthTemplate.Models;
using BlazorAuthTemplate.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthTemplate.Services
{
	public class BlogPostRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : IBlogPostRepository
	{
		public async Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			blogPost.Created = DateTime.Now;

			context.BlogPosts.Add(blogPost);
			await context.SaveChangesAsync();

			return blogPost;
		}

		public async Task DeleteBlogPostAsync(int blogPostId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			BlogPost? blogPost = await context.BlogPosts.FirstOrDefaultAsync(b => b.Id == blogPostId);

			if (blogPost != null) 
			{
				blogPost.IsPublished = false;
				blogPost.IsDeleted = true;
				await context.SaveChangesAsync();
			}
		}

		public async Task<BlogPost?> GetBlogPostByIdAsync(int id)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			BlogPost? blogPost = await context.BlogPosts
											.Where(b => b.IsPublished == true && b.IsDeleted == false)
											.Include(b => b.Category)
											.Include(b => b.Tags)
											.Include(b => b.Comments).ThenInclude(c => c.Author)
											.FirstOrDefaultAsync(b => b.Id == id);

			return blogPost;
		}

		public async Task<BlogPost?> GetBlogPostBySlugAsync(string slug)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			BlogPost? blogPost = await context.BlogPosts
											  .Where(b => b.IsPublished == true && b.IsDeleted == false)
											  .Include(b => b.Category)
											  .Include(b => b.Tags)
											  .Include(b => b.Comments).ThenInclude(c => c.Author)
											  .FirstOrDefaultAsync(b => b.Slug == slug);

			return blogPost;
		}

		public async Task<IEnumerable<BlogPost>> GetDeletedPostsAsync()
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			List<BlogPost> blogPosts = new List<BlogPost>();

			blogPosts = await context.BlogPosts
									.Where(b => b.IsDeleted == true)
									.Include(b => b.Category)
									.Include(b => b.Comments)
									.OrderByDescending(b => b.Created)
									.ToListAsync();
			return blogPosts;
		}

		public async Task<IEnumerable<BlogPost>> GetDraftPostsAsync()
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			List<BlogPost> blogPosts = new List<BlogPost>();

			blogPosts = await context.BlogPosts
									.Where(b => b.IsDeleted == false && b.IsPublished == false)
									.Include(b => b.Comments)
									.Include(b => b.Category)
									.OrderByDescending(b => b.Created)
									.ToListAsync();
			return blogPosts;
		}

		public async Task<IEnumerable<BlogPost>> GetPostsByCategoryIdAsync(int categoryId, int page, int pageSize)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			List<BlogPost> blogPosts = new List<BlogPost>();

			blogPosts = await context.BlogPosts
									.Where(b => b.IsPublished == true && b.IsDeleted == false && b.CategoryId == categoryId)
									.Include(b => b.Category)
									.Include(b => b.Comments)
									.OrderByDescending(b => b.Created)
									.ToListAsync();
			return blogPosts;
		}

		public async Task<IEnumerable<BlogPost>> GetPublishedPostsAsync(int page, int pageSize)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			IEnumerable<BlogPost> blogPosts = await context.BlogPosts
									.Where(b => b.IsPublished == true && b.IsDeleted == false)
									.Include(b => b.Category)
									.Include(b => b.Comments)
									.OrderByDescending(b => b.Created)	
									.ToListAsync();
			return blogPosts;
		}

		public async Task PublishBlogPostAsync(int blogPostId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			BlogPost? blogPost = await context.BlogPosts.FirstOrDefaultAsync(b => b.Id == blogPostId);

			if (blogPost != null)
			{
				blogPost.IsDeleted = false;
				blogPost.IsPublished = true;

				await context.SaveChangesAsync();
			}
		}

		public async Task RestoreBlogPostAsync(int blogPostId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			BlogPost? blogPost = await context.BlogPosts.FirstOrDefaultAsync(b => b.Id == blogPostId);

			if (blogPost != null)
			{
				blogPost.IsDeleted = false;

				await context.SaveChangesAsync();
			};
		}

		public async Task UnpublishBlogPostAsync(int blogPostId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			BlogPost? blogPost = await context.BlogPosts.FirstOrDefaultAsync(b => b.Id == blogPostId);

			if (blogPost != null)
			{
				blogPost.IsPublished = false;

				await context.SaveChangesAsync();
			}
		}

		public async Task UpdateBlogPostAsync(BlogPost blogPost)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			ImageUpload? oldImage = null;
			if(blogPost.Image is not null)
			{
				oldImage = await context.Images.FirstOrDefaultAsync(i => i.Id == blogPost.ImageId);

				blogPost.ImageId = blogPost.Image.Id;
				context.Images.Add(blogPost.Image);
			}
			
			//slug
			blogPost.Updated = DateTime.UtcNow;

			context.Update(blogPost);
			await context.SaveChangesAsync();

			if (oldImage is not null)
			{
				context.Images.Remove(oldImage);
				await context.SaveChangesAsync();
			}
		}
	}
}
