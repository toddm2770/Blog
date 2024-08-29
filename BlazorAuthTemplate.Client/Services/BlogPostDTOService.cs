using BlazorAuthTemplate.Client.Models;
using BlazorAuthTemplate.Client.Services.Interfaces;
using BlazorAuthTemplate.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace BlazorAuthTemplate.Client.Services
{
	public class BlogPostDTOService : IBlogPostDTOService
	{
		private readonly HttpClient _httpClient;

		public BlogPostDTOService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<PagedList<BlogPostDTO>> GetPublishedPostsAsync(int page, int pageSize)
		{
			var blogPosts = await _httpClient.GetFromJsonAsync<PagedList<BlogPostDTO>>($"api/blogposts") ?? null;
			return blogPosts!;
		}
	}
}
