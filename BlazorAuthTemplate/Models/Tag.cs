using BlazorAuthTemplate.Client.Models;
using Microsoft.Extensions.Hosting;

namespace BlazorAuthTemplate.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public int Name { get; set; }

        public virtual ICollection<BlogPost> Posts { get; set; } = [];
    }

    public static class TagExtension
    {
        public static TagDTO ToDTO(this Tag tag)
        {
            return new TagDTO()
            {
                Id = tag.Id,
                Name = tag.Name,
                Posts = [.. tag.Posts.Select(p => p.ToDTO())]
            };
        }
    }
}
