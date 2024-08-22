using BlazorAuthTemplate.Client.Helpers;
using System.ComponentModel.DataAnnotations;

namespace BlazorAuthTemplate.Client.Models
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public Guid? ImageId { get; set; }

        public string? ImageUrl { get; set; }

        public virtual ICollection<BlogPostDTO> Posts { get; set; } = [];
    }
}
