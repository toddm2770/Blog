using BlazorAuthTemplate.client.Services.Interfaces;
using BlazorAuthTemplate.Client;
using BlazorAuthTemplate.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;

namespace BlazorAuthTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        private UserInfo? userInfo;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetComments([FromQuery] int id)
        {
            try
            {
                IEnumerable<CommentDTO> comments = [];

                if (id != 0)
                {
                    comments = await _commentService.GetCommentsAsync(id);
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
        public async Task<ActionResult<CommentDTO>> GetCommentById(int id)
        {
            try
            {
                CommentDTO? comment = await _commentService.GetCommentByIdAsync(id);

                return comment == null ? NotFound() : Ok(comment);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }

        }

        [HttpPost]
        public async Task<ActionResult<CommentDTO>> CreateComment([FromBody] CommentDTO commentDTO)
        {
            try
            {
                commentDTO.AuthorId = userInfo!.UserId;
                commentDTO.Created = DateTime.UtcNow;

                CommentDTO createdComment = await _commentService.CreateCommentAsync(commentDTO);
                return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.Id });
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
