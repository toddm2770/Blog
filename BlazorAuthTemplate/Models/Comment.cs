using BlazorAuthTemplate.Client.Helpers;
using BlazorAuthTemplate.Client.Models;
using BlazorAuthTemplate.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace BlazorAuthTemplate.Models
{
    public class Comment
    {
        private DateTimeOffset _created;
        private DateTimeOffset _updated;

        public int Id { get; set; }

        [Required]
        [StringLength(5000, MinimumLength = 2)]
        public string? Content { get; set; }

        [Required]
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        public DateTimeOffset Updated
        {
            get => _updated;
            set => _updated = value.ToUniversalTime();
        }

        [MaxLength(200)]
        public string? UpdateReason { get; set; }

        [Required]
        public string? AuthorId { get; set; }
        public virtual ApplicationUser? Author { get; set; }

        
        public int BlogPostId { get; set; }
        public virtual BlogPost? BlogPost { get; set; }

        
    }

    public static class CommentExtension
    {
        public static CommentDTO ToDTO(this Comment comment)
        {
            CommentDTO commentDTO = new CommentDTO()
            {
                Id = comment.Id,
                Content = comment.Content,
                Created = comment.Created,
                Updated = comment.Updated,
                UpdateReason = comment.UpdateReason,
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author?.FullName,
                AuthorImageUrl = comment.Author?.ImageId is not null ? $"api/uploads/{comment.Author.ImageId}" : ImageHelper.DefaultProfilePicture,
                BlogPostId = comment.BlogPostId

            };

            return commentDTO;
        }
    }
}
