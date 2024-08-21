using BlazorAuthTemplate.Client.Models;

namespace BlazorAuthTemplate.client.Services.Interfaces
{
	public interface ICommentService
	{
		Task<CommentDTO> CreateCommentAsync(CommentDTO commentDTO);
		Task UpdateCommentAsync(CommentDTO commentDTO);
		Task DeleteCommentAsync(int commentId);

		Task<CommentDTO?> GetCommentByIdAsync(int commentId);
		Task<IEnumerable<CommentDTO>> GetCommentsAsync(int postId);
	}
}
