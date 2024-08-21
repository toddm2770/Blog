
namespace BlazorAuthTemplate.Client.Models
{
    public class TagDTO
    {
        public int Id { get; set; }

        public int Name { get; set; }

        public virtual ICollection<BlogPostDTO>? Posts { get; set; } = [];
    }
}
