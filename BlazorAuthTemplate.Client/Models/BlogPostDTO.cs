using BlazorAuthTemplate.Client.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlazorAuthTemplate.Client.Models
{
    public class BlogPostDTO
    {
        private DateTimeOffset _created;
        private DateTimeOffset _updated;

        public int Id { get; set; }

        [Required(ErrorMessage = "Every task must have a title.")]
        public string? Title { get; set; }

        //[Required]
        public string? Slug { get; set; }

        [Required]
        [StringLength(600, MinimumLength = 2)]
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

        public string? ImageUrl { get; set; } = ImageHelper.DefaultProfilePicture;

        public virtual ICollection<CommentDTO>? Comments { get; set; } = [];

        public virtual ICollection<TagDTO>? Tags { get; set; } = [];

        public virtual CategoryDTO? Category { get; set; }

        public int CategoryId { get; set; }
    }
}
