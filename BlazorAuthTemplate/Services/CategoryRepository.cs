using BlazorAuthTemplate.Data;
using BlazorAuthTemplate.Models;
using BlazorAuthTemplate.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthTemplate.Services
{
	public class CategoryRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : ICategoryRepository
	{
		public async Task<Category> CreateCategoryAsync(Category category)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			context.Categories.Add(category);
			await context.SaveChangesAsync();

			return category;
		}

		public async Task DeleteCategoryAsync(int categoryId)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			Category? category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

			if (category != null)
			{
				context.Categories.Remove(category);

				if (category.Image is not null)
				{
					context.Images.Remove(category.Image);
				}

				await context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Category>> GetCategoriesAsync()
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			List<Category> categories = await context.Categories
										  .Include(c => c.Posts)
										  .ToListAsync();
			return categories;


		}

		public async Task<Category?> GetCategoryByIdAsync(int id)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			Category? category = await context.Categories
										.Include(c => c.Posts)
										.FirstOrDefaultAsync(c => c.Id == id);

			return category;
		}

		public async Task UpdateCategoryAsync(Category category)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			ImageUpload? existingImage = null;

			if (category.Image is not null)
			{
				existingImage = await context.Images.FirstOrDefaultAsync(i => i.Id == category.ImageId);

				category.ImageId = category.Image.Id;
				context.Images.Add(category.Image);
			}

			context.Categories.Add(category);
			await context.SaveChangesAsync();

			if(existingImage is not null)
			{
				context.Remove(existingImage);
				await context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Category>> GetTopCategoriesAsync(int count)
		{
			using ApplicationDbContext context = contextFactory.CreateDbContext();

			List<Category> categories = await context.Categories
											.Include(c => c.Posts.Where(p => p.IsPublished == true && p.IsDeleted == false))
											.OrderByDescending(c => c.Posts.Where(p => p.IsPublished == true && p.IsDeleted == false).Count())
											.Take(count)
											.ToListAsync();
			return categories;				
									
		}
	}
}
