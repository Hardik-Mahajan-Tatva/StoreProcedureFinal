using PizzaShop.Repository.Models;
using Pizzashop.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Pizzashop.Repository.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public CategoryRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region IsCategoryNameExistsAsync
        public async Task<bool> IsCategoryNameExistsAsync(string categoryName)
        {
            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Categoryname == categoryName && (c.Isdeleted == false));
        }
        #endregion

        #region AddCategoryAsync
        public async Task<int> AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category.Categoryid;
        }
        #endregion

        #region SoftDeleteCategoryAsync
        public async Task<bool> SoftDeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                category.Isdeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        #endregion

        #region GetAllCategoriesAsync
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.Isdeleted == false)
                .OrderBy(c => c.Sortorder)
                .ToListAsync();
        }
        #endregion

        #region SPGetAllCategoriesAsync
        public async Task<List<Category>> SPGetAllCategoriesAsync()
        {

            List<Category> listCateogry = await _context.Categories
        .FromSqlRaw("SELECT * FROM get_all_categories()")
        .ToListAsync();
            return listCateogry;
        }
        #endregion

        #region GetCategoryByIdAsync
        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Categoryid == categoryId);
        }
        #endregion

        #region DoesCategoryExistAsync
        public async Task<bool> DoesCategoryExistAsync(string categoryName, int excludeCategoryId)
        {
            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Categoryname == categoryName && c.Categoryid != excludeCategoryId);
        }
        #endregion

        #region UpdateCategoryAsync
        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region SaveChangesAsync
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetCategoryNameByCategoryId
        public async Task<string?> GetCategoryNameByCategoryId(int categoryId)
        {
            return await _context.Categories
                .Where(c => c.Categoryid == categoryId)
                .Select(c => c.Categoryname)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        #endregion
        #region SPGetCategoryNameByCategoryId
        public async Task<string?> SPGetCategoryNameByCategoryId(int categoryId)
        {
            var result = await _context.Categories
        .FromSqlRaw("SELECT get_category_name_by_id({0}) AS categoryname", categoryId)
        .Select(x => x.Categoryname)
        .FirstOrDefaultAsync();

            return result;
        }
        #endregion
    }
}

