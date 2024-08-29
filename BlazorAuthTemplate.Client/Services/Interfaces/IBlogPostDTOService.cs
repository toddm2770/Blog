using BlazorAuthTemplate.Client.Models;
using BlazorAuthTemplate.Models;

namespace BlazorAuthTemplate.Client.Services.Interfaces
{
	public interface IBlogPostDTOService
	{
		Task<PagedList<BlogPostDTO>> GetPublishedPostsAsync(int page, int pageSize);
	}
}
