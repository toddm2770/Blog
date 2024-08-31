using BlazorAuthTemplate.Data;
using BlazorAuthTemplate.Helpers.Extensions;
using BlazorAuthTemplate.Models;
using BlazorAuthTemplate.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BlazorAuthTemplate.Services
{
	public class BlogPostRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : IBlogPostRepository
	{
		public async Task AddTagsToBlogPostAsync(int blogPostId, IEnumerable<string> tagNames)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			TextInfo textInfo = new CultureInfo("en-US").TextInfo;

			BlogPost? blogPost = await context.BlogPosts
											  .Include(b => b.Tags)
											  .FirstOrDefaultAsync(b => b.Id == blogPostId);

			if (blogPost != null)
			{
				foreach (var tagName in tagNames) 
				{
					try
					{
					Tag? existingTag = await context.Tags.FirstOrDefaultAsync(t => t.Name!.Trim().ToLower() == tagName.Trim().ToLower());

					if (existingTag != null)
					{
						blogPost.Tags.Add(existingTag);
					}
					else 
					{
						string titleCaseTagName = textInfo.ToTitleCase(tagName.Trim());

						Tag newTag = new Tag(){ Name = titleCaseTagName };

						context.Tags.Add(newTag);
						blogPost.Tags.Add(newTag);
					}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						throw;
					}

				}

				await context.SaveChangesAsync();

			}
		}
		public async Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			blogPost.Created = DateTime.Now;

            blogPost.Slug = await GenerateSlugAsync(blogPost.Title!, blogPost.Id);

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
											  .Where(b => b.IsPublished == true && b.IsDeleted == false || b.IsPublished == false && b.IsDeleted == false)
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

		public async Task<IEnumerable<BlogPost>> GetPostsByTagIdAsync(int tagId, int page, int pageSize)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			List<BlogPost> blogPosts = new List<BlogPost>();

			blogPosts = await context.BlogPosts
									 .Where(b => b.IsPublished == true && b.IsDeleted == false)
									 .Include(b => b.Category)
									 .Include(b => b.Comments)
									 .Include(b => b.Tags)
									 .Where(b => b.Tags.Any(t => t.Id == tagId))
									 .OrderByDescending(b => b.Created)
									 .ToListAsync();
			return blogPosts;
		}

		public async Task<PagedList<BlogPost>> GetPublishedPostsAsync(int page, int pageSize)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			PagedList<BlogPost> blogPosts = await context.BlogPosts
									                       .Where(b => b.IsPublished == true && b.IsDeleted == false)
														   .Include(b => b.Category)
														   .Include(b => b.Comments)
														   .OrderByDescending(b => b.Created)	
														   .ToPagedListAsync(page, pageSize);
			return blogPosts;
		}

		public async Task<Tag?> GetTagByIdAsync(int tagId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			Tag? tag =await context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);

			return tag;
		}

		public async Task<IEnumerable<BlogPost>> GetTopBlogPostsAsync(int count)
        {
			using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<BlogPost> blogPosts = await context.BlogPosts
													.Where(b => b.IsPublished == true && b.IsDeleted == false)
													.Include(b => b.Comments)
													.Include (b => b.Category)
													.OrderByDescending(b => b.Comments.Count())
													.Take(count)
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

		public async Task RemoveTagsFromBlogPostAsync(int blogPostId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			BlogPost? blogPost = await context.BlogPosts
											  .Include(b => b.Tags)
											  .FirstOrDefaultAsync(b => b.Id == blogPostId);

			if(blogPost != null)
			{
				blogPost.Tags.Clear();
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

        public async Task<PagedList<BlogPost>> SearchBlogPostsAsync(string query, int page, int pageSize)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

			string normalizedQuery = query.Trim().ToLower();

			PagedList<BlogPost> results = await context.BlogPosts
					.Where(b => b.IsPublished == true && b.IsDeleted == false)
					.Include(b => b.Tags)
					.Include(b => b.Category)
					.Include(b => b.Comments)
					.ThenInclude(c => c.Author)
					.Where(b => string.IsNullOrWhiteSpace(normalizedQuery)
					|| b.Title!.ToLower().Contains(normalizedQuery)
					|| b.Abstract!.ToLower().Contains(normalizedQuery)
					|| b.Content!.ToLower().Contains(normalizedQuery)
					|| b.Category!.Name!.ToLower().Contains(normalizedQuery)
					|| b.Tags!.Select(t => t.Name!.ToLower()).Any(tagName => tagName.Contains(normalizedQuery))
					|| b.Comments!.Any(c => c.Content!.ToLower().Contains(normalizedQuery)
										|| c.Author!.FirstName!.ToLower().Contains(normalizedQuery)
										|| c.Author.LastName!.ToLower().Contains(normalizedQuery))
					)
					.OrderByDescending(b => b.Created)
					.ToPagedListAsync(page, pageSize);
			return results;
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

            blogPost.Slug = await GenerateSlugAsync(blogPost.Title!, blogPost.Id);

            blogPost.Updated = DateTimeOffset.UtcNow;

			context.Update(blogPost);
			await context.SaveChangesAsync();

			if (oldImage is not null)
			{
				context.Images.Remove(oldImage);
				await context.SaveChangesAsync();
			}
		}

        #region slugs
        private async Task<string> GenerateSlugAsync(string title, int id)
        {
            if (await ValidateSlugAsync(title, id))
            {
                return Slugify(title); // "my-first-post"
            }
            else
            {
                int i = 2;
                string newTitle = $"{title} {i}"; // "my-first-post-2"
                bool isValid = await ValidateSlugAsync(newTitle, id);

                while (isValid == false)
                {
                    i++;
                    newTitle = $"{title} {i}"; // "my-first-post-3"
                    isValid = await ValidateSlugAsync(newTitle, id);
                }

                return Slugify(newTitle);
            }
        }

        private async Task<bool> ValidateSlugAsync(string title, int blogId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            string newSlug = Slugify(title);

            bool isValid = false;
            if (blogId == 0)
            {
                // this is a new post, so just check if anyone has this slug
                isValid = !await context.BlogPosts.AnyAsync(bp => bp.Slug == newSlug);
            }
            else
            {
                // this is an existing post, so see if any OTHER posts have this slug
                isValid = !await context.BlogPosts.AnyAsync(bp => bp.Slug == newSlug && bp.Id != blogId);
            }

            return isValid;
        }

        private static string Slugify(string title)
        {
            if (string.IsNullOrEmpty(title)) return title;

            title = title.Normalize(System.Text.NormalizationForm.FormD);
            char[] chars = title.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();

            string normalizedTitle = new string(chars).Normalize(System.Text.NormalizationForm.FormC)
                                                      .ToLower()
                                                      .Trim();

            string titleWithoutSymbols = Regex.Replace(normalizedTitle, @"[^A-Za-z0-9\s-]", "");
            string slug = Regex.Replace(titleWithoutSymbols, @"\s+", "-");

            return slug;
        }
        #endregion
    }
}
