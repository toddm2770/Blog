using BlazorAuthTemplate.Client.Models;
using BlazorAuthTemplate.Models;
using BlazorAuthTemplate.Services.Interfaces;
using BlazorAuthTemplate.client.Services.Interfaces;
using BlazorAuthTemplate.Helpers;

namespace BlazorAuthTemplate.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly ICategoryRepository _repository;

		public CategoryService(ICategoryRepository repository) 
		{
			_repository = repository;
		}

		public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDTO)
		{
			Category newCategory = new()
			{
				Name = categoryDTO.Name,
				Description = categoryDTO.Description,
			};

			if (categoryDTO.ImageUrl!.StartsWith("data:") == true)
			{
				newCategory.Image = UploadHelper.GetImageUpload(categoryDTO.ImageUrl);
			}
			Category createdCategory = await _repository.CreateCategoryAsync(newCategory);

			return createdCategory.ToDTO();
		}

		public async Task DeleteCategoryAsync(int categoryId) =>
			await _repository.DeleteCategoryAsync(categoryId);
		

		public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
		{
			IEnumerable<Category> categories = await _repository.GetCategoriesAsync();

			return categories.Select(c => c.ToDTO());
		}

		public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
		{
			Category? category = await _repository.GetCategoryByIdAsync(id);
			return category?.ToDTO();
		}

		public async Task UpdateCategoryAsync(CategoryDTO categoryDTO)
		{
			var categoryToUpdate = await _repository.GetCategoryByIdAsync(categoryDTO.Id);
			if (categoryToUpdate is null)
			{
				return;
			}

			categoryToUpdate.Name = categoryDTO.Name;
			categoryToUpdate.Description = categoryDTO.Description;

			if (categoryDTO.ImageUrl.StartsWith("data:"))
			{
				categoryToUpdate.Image = UploadHelper.GetImageUpload(categoryDTO.ImageUrl);
			}
			else
			{
				categoryToUpdate.Image = null;
			}

			await _repository.UpdateCategoryAsync(categoryToUpdate);
		}
	}
}
