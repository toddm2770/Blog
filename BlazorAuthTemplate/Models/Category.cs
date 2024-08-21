using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlazorAuthTemplate.Client.Models;
using BlazorAuthTemplate.Helpers;

namespace BlazorAuthTemplate.Models;
public class Category
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    public Guid? ImageId { get; set; }

    public virtual ImageUpload? Image { get; set; }

    [NotMapped]
    public string ImageUrl => ImageId.HasValue ? $"api/uploads/{ImageId}" : UploadHelper.DefaultCategoryImage;

    public virtual ICollection<BlogPost> Posts { get; set; } = [];
}

public static class CategoryExtension
{
    public static CategoryDTO ToDTO(this Category category)
    {
        return new CategoryDTO()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl, 
            Posts = [.. category.Posts.Select(p => p.ToDTO())]
        };
    }
}
