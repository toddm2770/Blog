using BlazorAuthTemplate.Client.Models;
using BlazorAuthTemplate.Client.Services.Interfaces;
using BlazorAuthTemplate.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAuthTemplate.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BlogPostDTOController : ControllerBase
	{
		private readonly IBlogPostDTOService _blogPostDTOService;

		public BlogPostDTOController(IBlogPostDTOService blogPostDTOService)
		{
			_blogPostDTOService = blogPostDTOService;
		}

		[HttpGet]
		public async Task<ActionResult<PagedList<BlogPostDTO>>> GetBlogPosts([FromQuery] int page, int pageSize)
		{
			try
			{
				var blogPosts = await _blogPostDTOService.GetPublishedPostsAsync(page, pageSize);
				if (blogPosts == null)
				{
					return NotFound();
				}

				return Ok(blogPosts);
			}
			catch (Exception ex) 
			{
				Console.WriteLine(ex);
				throw;
			}
		}
	}
}
