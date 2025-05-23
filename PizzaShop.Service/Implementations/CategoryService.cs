using Pizzashop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<int> AddAsync(CategoryViewModel categoryViewModel)
        {

            bool exists = await _categoryRepository.IsCategoryNameExistsAsync(categoryViewModel.CategoryName ?? string.Empty.ToLower());
            if (exists)
            {
                throw new InvalidOperationException("A category with the same name already exists.");
            }

            Category category = new()
            {
                Categoryname = categoryViewModel.CategoryName ?? string.Empty,
                Description = categoryViewModel.Description
            };
            return await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            if (categoryId == 0)
            {
                return false;
            }
            else
            {
                var deleted = await _categoryRepository.SoftDeleteCategoryAsync(categoryId);
                return deleted;
            }
        }

        public async Task<List<CategoryViewModel>> GetAll()
        {
            List<Category> categoryDb = await _categoryRepository.GetAllCategoriesAsync();
            return categoryDb.Select(item => new CategoryViewModel()
            {
                CategoryId = item.Categoryid,
                CategoryName = item.Categoryname,
                Description = item.Description
            }).ToList();
        }

        public async Task<CategoryViewModel> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return new CategoryViewModel();
            }

            return new CategoryViewModel
            {
                CategoryId = category.Categoryid,
                CategoryName = category.Categoryname,
                Description = category.Description
            };
        }

        public async Task<bool> UpdateAsync(CategoryViewModel category)
        {
            var entity = await _categoryRepository.GetCategoryByIdAsync(category.CategoryId);
            if (entity == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            bool isDuplicate = await _categoryRepository.DoesCategoryExistAsync(category.CategoryName ?? string.Empty, category.CategoryId);
            if (isDuplicate)
            {
                throw new InvalidOperationException("A category with the same name already exists.");
            }

            entity.Categoryname = category.CategoryName ?? string.Empty;
            entity.Description = category.Description;

            return await _categoryRepository.UpdateCategoryAsync(entity);
        }
        public async Task UpdateCategoryOrderAsync(List<int> orderedCategoryIds)
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();

            foreach (var (categoryId, index) in orderedCategoryIds.Select((id, idx) => (id, idx)))
            {
                var category = categories.FirstOrDefault(c => c.Categoryid == categoryId);
                if (category != null)
                {
                    category.Sortorder = index + 1;
                }
            }

            await _categoryRepository.SaveChangesAsync();
        }
        public async Task<string?> GetCategoryNameByCategoryId(int categoryId)
        {
            return await _categoryRepository.GetCategoryNameByCategoryId(categoryId);
        }


    }
}
