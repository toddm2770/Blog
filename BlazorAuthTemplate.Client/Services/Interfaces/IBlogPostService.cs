using BlazorAuthTemplate.Client.Models;

namespace BlazorAuthTemplate.client.Services.Interfaces
{
	public interface IBlogPostService
	{
		Task<BlogPostDTO> CreateBlogPostAsync(BlogPostDTO blogPost);
		Task UpdateBlogPostAsync(BlogPostDTO blogPost);

		Task<IEnumerable<BlogPostDTO>> GetPublishedPostsAsync();
		Task<IEnumerable<BlogPostDTO>> GetPostsByCategoryIdAsync(int categoryId);
		Task<IEnumerable<BlogPostDTO>> GetDraftPostsAsync();
		Task<IEnumerable<BlogPostDTO>> GetDeletedPostsAsync();
		Task<BlogPostDTO?> GetBlogPostBySlugAsync(string slug);
		Task<BlogPostDTO?> GetBlogPostByIdAsync(int id);

		Task DeleteBlogPostAsync(int blogPostId);
		Task RestoreBlogPostAsync(int blogPostId);
		Task PublishBlogPostAsync(int blogPostId);
		Task UnpublishBlogPostAsync(int blogPostId);
		Task<IEnumerable<BlogPostDTO>> SearchBlogPostsAsync(string query);
		Task<IEnumerable<BlogPostDTO>> GetTopBlogPostsAsync(int count);

		Task<TagDTO?> GetTagByIdAsync(int id);
		Task<IEnumerable<BlogPostDTO>> GetPostsByTagIdAsync(int tagId, int page, int pageSize);

	}

}
