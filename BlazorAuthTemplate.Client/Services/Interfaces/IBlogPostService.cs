﻿using BlazorAuthTemplate.Client.Models;
using BlazorAuthTemplate.Models;
using System.Net.NetworkInformation;
using System.Reflection;

namespace BlazorAuthTemplate.client.Services.Interfaces
{
    public interface IBlogPostService
	{
		Task<BlogPostDTO> CreateBlogPostAsync(BlogPostDTO blogPost);
		Task UpdateBlogPostAsync(BlogPostDTO blogPost);

		Task<PagedList<BlogPostDTO>> GetPublishedPostsAsync(int page, int pageSize);
		Task<IEnumerable<BlogPostDTO>> GetPostsByCategoryIdAsync(int categoryId);
		Task<IEnumerable<BlogPostDTO>> GetDraftPostsAsync();
		Task<IEnumerable<BlogPostDTO>> GetDeletedPostsAsync();
		Task<BlogPostDTO?> GetBlogPostBySlugAsync(string slug);
		Task<BlogPostDTO?> GetBlogPostByIdAsync(int id);

		Task DeleteBlogPostAsync(int blogPostId);
		Task RestoreBlogPostAsync(int blogPostId);
		Task PublishBlogPostAsync(int blogPostId);
		Task UnpublishBlogPostAsync(int blogPostId);
		Task<PagedList<BlogPostDTO>> SearchBlogPostsAsync(string query, int page, int pageSize);
		Task<IEnumerable<BlogPostDTO>> GetTopBlogPostsAsync(int count);

		Task<TagDTO?> GetTagByIdAsync(int id);
		Task<IEnumerable<BlogPostDTO>> GetPostsByTagIdAsync(int tagId, int page, int pageSize);

	}

}
