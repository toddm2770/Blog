using BlazorAuthTemplate.Client.Models;

namespace BlazorAuthTemplate.client.Services.Interfaces
{
	public interface ICategoryService
	{
		Task<CategoryDTO> CreateCategoryAsync(CategoryDTO category);
		Task<IEnumerable<CategoryDTO>> GetCategoriesAsync();
		Task<CategoryDTO?> GetCategoryByIdAsync(int id);
		Task UpdateCategoryAsync(CategoryDTO category);
		Task DeleteCategoryAsync(int categoryId);
		Task<IEnumerable<CategoryDTO>> GetTopCategoriesAsync(int count);
	}
}
