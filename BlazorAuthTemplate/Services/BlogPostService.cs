using BlazorAuthTemplate.Client.Models;
using BlazorAuthTemplate.Models;
using BlazorAuthTemplate.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;
using BlazorAuthTemplate.client.Services.Interfaces;
using BlazorAuthTemplate.Helpers;

namespace BlazorAuthTemplate.Services
{
	public class BlogPostService : IBlogPostService
	{

		private readonly IBlogPostRepository _repository;

		public BlogPostService(IBlogPostRepository repository)
		{
			_repository = repository;
		}

		public async Task<BlogPostDTO> CreateBlogPostAsync(BlogPostDTO blogPostDTO)
		{
			BlogPost newBlogPost = new()
			{
				Title = blogPostDTO.Title,
				Abstract = blogPostDTO.Abstract,
				Content = blogPostDTO.Content,
				IsPublished = blogPostDTO.IsPublished,
				IsDeleted = blogPostDTO.IsDeleted,
				Created = DateTimeOffset.Now,
				CategoryId = blogPostDTO.CategoryId
			};

			if(blogPostDTO.ImageUrl?.StartsWith("data:") == true)
			{
				try
				{
					newBlogPost.Image = UploadHelper.GetImageUpload(blogPostDTO.ImageUrl);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}

			newBlogPost = await _repository.CreateBlogPostAsync(newBlogPost);

			IEnumerable<string> tagNames = blogPostDTO.Tags!.Select(t => t.Name!);

			await _repository.AddTagsToBlogPostAsync(newBlogPost.Id, tagNames);

			return newBlogPost.ToDTO();
		}

		public async Task DeleteBlogPostAsync(int blogPostId) =>		
			await _repository.DeleteBlogPostAsync(blogPostId);
		

		public async Task<BlogPostDTO?> GetBlogPostByIdAsync(int id)
		{
			BlogPost? blogPost = await _repository.GetBlogPostByIdAsync(id);

			return blogPost?.ToDTO();
		}

		public async Task<BlogPostDTO?> GetBlogPostBySlugAsync(string slug)
		{
			BlogPost? blogPost = await _repository.GetBlogPostBySlugAsync(slug);

			return blogPost?.ToDTO();
		}

		public async Task<IEnumerable<BlogPostDTO>> GetDeletedPostsAsync()
		{
			IEnumerable<BlogPost> blogPost = await _repository.GetDeletedPostsAsync();

			return blogPost.Select(b => b.ToDTO());
		}

		public async Task<IEnumerable<BlogPostDTO>> GetDraftPostsAsync()
		{
			IEnumerable<BlogPost> blogPost = await _repository.GetDraftPostsAsync();

			return blogPost.Select(b => b.ToDTO());
		}

		public async Task<IEnumerable<BlogPostDTO>> GetPostsByCategoryIdAsync(int categoryId)
		{
			IEnumerable<BlogPost> blogPost = await _repository.GetPostsByCategoryIdAsync(categoryId, 0, 0);

			IEnumerable<BlogPostDTO> dtos = blogPost.Select(p => p.ToDTO());

			return dtos;
		}

		public async Task<IEnumerable<BlogPostDTO>> GetPostsByTagIdAsync(int tagId, int page, int pageSize)
		{
			IEnumerable<BlogPost> blogPost = await _repository.GetPostsByTagIdAsync(tagId, 0, 0);

			IEnumerable<BlogPostDTO> dtos = blogPost.Select(p =>p.ToDTO());

			return dtos;
		}

		public async Task<IEnumerable<BlogPostDTO>> GetPublishedPostsAsync()
		{
			IEnumerable<BlogPost> blogPost = await _repository.GetPublishedPostsAsync(0, 0);

			IEnumerable<BlogPostDTO> dtos = blogPost.Select(propa => propa.ToDTO());

			List<BlogPostDTO> blogPostDTOs = [];

			foreach(var post in blogPost)
			{
				blogPostDTOs.Add(post.ToDTO());
			}

			dtos = blogPostDTOs;

			return dtos;
		}

		public async Task<TagDTO?> GetTagByIdAsync(int id)
		{
			Tag? tag = await _repository.GetTagByIdAsync(id);

			return tag?.ToDTO();
		}

		public async Task<IEnumerable<BlogPostDTO>> GetTopBlogPostsAsync(int count)
        {
            IEnumerable<BlogPost> blogPosts = await _repository.GetTopBlogPostsAsync(count);

            return blogPosts.Select(c => c.ToDTO());
        }

        public async Task PublishBlogPostAsync(int blogPostId) =>	
			await _repository.PublishBlogPostAsync(blogPostId);
		

		public async Task RestoreBlogPostAsync(int blogPostId) =>		
			await _repository.RestoreBlogPostAsync(blogPostId);

        public async Task<IEnumerable<BlogPostDTO>> SearchBlogPostsAsync(string query)
        {
			IEnumerable<BlogPost> posts = await _repository.SearchBlogPostsAsync(query);

			return posts.Select(p => p.ToDTO());
        }

        public async Task UnpublishBlogPostAsync(int blogPostId) =>		
			await _repository.UnpublishBlogPostAsync(blogPostId);
		

		public async Task UpdateBlogPostAsync(BlogPostDTO blogPost)
		{
			await _repository.RemoveTagsFromBlogPostAsync(blogPost.Id);

			BlogPost? originalPost = await _repository.GetBlogPostByIdAsync(blogPost.Id);

			if (originalPost is null) { return; }
			
				originalPost.Title = blogPost.Title;
				originalPost.Slug = blogPost.Slug;
				originalPost.Abstract = blogPost.Abstract;
				originalPost.Content = blogPost.Content;
				originalPost.IsPublished = blogPost.IsPublished;
				originalPost.IsDeleted = blogPost.IsDeleted;
				originalPost.CategoryId = blogPost.CategoryId;
				originalPost.Category = null;
				originalPost.Updated = blogPost.Updated;

				if (blogPost.ImageUrl?.StartsWith("data:") == true)
				{
					try
					{
						originalPost.Image = UploadHelper.GetImageUpload(blogPost.ImageUrl);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
				}
				else originalPost.Image = null;

				await _repository.UpdateBlogPostAsync(originalPost);		

				IEnumerable<string> tagNames = blogPost.Tags.Select(t => t.Name);

				await _repository.AddTagsToBlogPostAsync(originalPost.Id, tagNames);
		}
	}
}
