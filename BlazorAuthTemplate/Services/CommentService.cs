using BlazorAuthTemplate.Client.Helpers;
using BlazorAuthTemplate.Client.Models;
using BlazorAuthTemplate.Models;
using BlazorAuthTemplate.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using BlazorAuthTemplate.client.Services.Interfaces;

namespace BlazorAuthTemplate.Services
{
	public class CommentService : ICommentService
	{
		private readonly ICommentRepository _repository;

		public CommentService(ICommentRepository repository)
		{
			_repository = repository;
		}

		public async Task<CommentDTO> CreateCommentAsync(CommentDTO commentDTO)
		{
			Comment newComment = new ()
			{
				Content = commentDTO.Content,
				Created = DateTimeOffset.UtcNow,
				AuthorId = commentDTO.AuthorId,
				BlogPostId = commentDTO.BlogPostId

			};

			newComment = await _repository.CreateCommentAsync(newComment);

			return newComment.ToDTO();

		}

		public async Task DeleteCommentAsync(int commentId) =>
			await _repository.DeleteCommentAsync(commentId);
		

		public async Task<CommentDTO?> GetCommentByIdAsync(int commentId)
		{
			Comment? comment = await _repository.GetCommentByIdAsync(commentId);

			return comment?.ToDTO();
		}

		public async Task<IEnumerable<CommentDTO>> GetCommentsAsync(int postId)
		{
			IEnumerable<Comment> comments = await _repository.GetCommentsAsync(postId);

			return comments.Select(c => c.ToDTO());
		}

		public async Task UpdateCommentAsync(CommentDTO commentDTO)
		{
			Comment? comment = await _repository.GetCommentByIdAsync(commentDTO.Id);

			if(comment is not null)
			{
				comment.Content = commentDTO.Content;
				comment.Updated = DateTimeOffset.UtcNow;
				comment.UpdateReason = commentDTO.UpdateReason;

				await _repository.UpdateCommentAsync(comment);
			}
		}
	}
}
