using BlazorAuthTemplate.client.Services.Interfaces;
using BlazorAuthTemplate.Client;
using BlazorAuthTemplate.Client.Models;
using BlazorAuthTemplate.Components.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;

namespace BlazorAuthTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

		private string _userId => User.GetUserId()!;

		public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        private UserInfo? userInfo;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetComments([FromQuery] int blogPostId)
        {
            try
            {
                IEnumerable<CommentDTO> comments = [];

                if (blogPostId != 0)
                {
                    comments = await _commentService.GetCommentsAsync(blogPostId);
                }

                return Ok(comments);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CommentDTO>> GetCommentById(int commentId)
        {
            try
            {
                CommentDTO? comment = await _commentService.GetCommentByIdAsync(commentId);

                return comment == null ? NotFound() : Ok(comment);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDTO>> CreateComment([FromBody] CommentDTO commentDTO)
        {
            try
            {
                if (commentDTO.BlogPostId == 0) return BadRequest();

                string userId = _userId;

                commentDTO.AuthorId = userId;
                commentDTO.Created = DateTime.UtcNow;

                CommentDTO createdComment = await _commentService.CreateCommentAsync(commentDTO);
                return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id }, createdComment);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateComment([FromRoute] int id, [FromBody] CommentDTO comment)
        {

            if(id != comment.Id)
            {
                return BadRequest();
            }
            try
            {
                await _commentService.UpdateCommentAsync(comment);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteContact([FromRoute] int id)
        {
            try
            {
                await _commentService.DeleteCommentAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
