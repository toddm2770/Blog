using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using BlazorAuthTemplate.Client.Models;

namespace BlazorAuthTemplate.Models
{
    public class BlogPost
    {
        private DateTimeOffset _created;
        private DateTimeOffset _updated;

        //Primary Key
        public int Id { get; set; }

        [Required(ErrorMessage = "Every task must have a title.")]
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(200, ErrorMessage = "The {0} must not exceed {1} characters.")]
        public string? Title { get; set; }

        [Required]
        public string? Slug { get; set; }

        [Required]
        [StringLength(600, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string? Abstract { get; set; }

        [Required]
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

        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }

        //Relationships
        public Guid? ImageId { get; set; }
        public virtual ImageUpload? Image {  get; set; }

        [NotMapped]
        public string? ImageUrl => ImageId.HasValue ? $"api/uploads/{ImageId}" : null;

        //Many to One
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        //One to Many
        public virtual ICollection<Comment> Comments { get; set; } = [];
        public virtual ICollection<Tag> Tags { get; set; } = [];
    }

    public static class BlogPostExtension
    {
        public static BlogPostDTO ToDTO(this BlogPost post)
        {
            ICollection<TagDTO> tags = [.. post.Tags.Select(t =>
            {
                t.Posts = [];
                return t.ToDTO();
            }
            )];

            CategoryDTO? category = null;

            if (post.Category != null)
            {
                post.Category.Posts = [];
                category = post.Category.ToDTO();
            }

            return new BlogPostDTO()
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Abstract = post.Abstract,
                Content = post.Content,
                IsPublished = post.IsPublished,
                IsDeleted = post.IsDeleted,
                CategoryId = post.CategoryId,
                Category = category,
                Comments = [.. post.Comments.Select(c => c.ToDTO())],
                Tags = tags,
                Created = post.Created,
                Updated = post.Updated,
                ImageUrl = post.ImageUrl
            };

        }

    }
}
