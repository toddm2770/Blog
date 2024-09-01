using BlazorAuthTemplate.Client.Helpers;
using System.ComponentModel.DataAnnotations;

namespace BlazorAuthTemplate.Client.Models
{
    public class CommentDTO
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;

        public int Id { get; set; }

        [Required]
        [StringLength(5000, MinimumLength = 2)]
        public string? Content { get; set; }

        [Required]
        [Display(Name = "Date Created")]
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        [Display(Name = "Date Updated")]
        public DateTimeOffset? Updated
        {
            get => _updated;
            set => _updated = value?.ToUniversalTime();
        }

        [StringLength(200, MinimumLength = 2)]
        public string? UpdateReason { get; set; }


        public string? Author { get; set; }
        public string? AuthorId { get; set; }

        public string? AuthorName { get; set; }

        public string? AuthorImageUrl { get; set; }

        public string? ImageUrl { get; set; } = ImageHelper.DefaultProfilePicture;

        public int BlogPostId { get; set; }
		public virtual BlogPostDTO? BlogPost { get; set; }


	}
}
