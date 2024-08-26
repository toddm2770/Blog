using BlazorAuthTemplate.Models;

namespace BlazorAuthTemplate.Services.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost);
        Task UpdateBlogPostAsync(BlogPost blogPost);

        Task<IEnumerable<BlogPost>> GetPublishedPostsAsync(int page, int pageSize);
        Task<IEnumerable<BlogPost>> GetDraftPostsAsync();
        Task<IEnumerable<BlogPost>> GetDeletedPostsAsync();
        Task<BlogPost?> GetBlogPostBySlugAsync(string slug);
        Task<BlogPost?> GetBlogPostByIdAsync(int id);
        Task<IEnumerable<BlogPost>> GetTopBlogPostsAsync(int count);
        Task<IEnumerable<BlogPost>> GetPostsByCategoryIdAsync(int categoryId, int page, int pageSize);
        Task<IEnumerable<BlogPost>> SearchBlogPostsAsync(string query, int page, int pageSize);

        Task DeleteBlogPostAsync(int blogPostId);
        Task RestoreBlogPostAsync(int blogPostId);
        Task PublishBlogPostAsync(int blogPostId);
        Task UnpublishBlogPostAsync(int blogPostId);

        Task AddTagsToBlogPostAsync(int blogPostId, IEnumerable<string> tagNames);
        Task RemoveTagsFromBlogPostAsync(int blogPostId);
        Task<Tag?> GetTagByIdAsync(int tagId);
        Task<IEnumerable<BlogPost>> GetPostsByTagIdAsync(int tagId, int page, int pageSize);

    }

}
