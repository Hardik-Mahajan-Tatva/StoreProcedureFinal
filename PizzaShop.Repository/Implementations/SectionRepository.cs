using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using Section = PizzaShop.Repository.Models.Section;

namespace PizzaShop.Repository.Implementations
{
    public class SectionRepository : ISectionRepository
    {
        private readonly PizzaShopContext _context;

        #region Constructor
        public SectionRepository(PizzaShopContext context)
        {
            _context = context;
        }
        #endregion

        #region GetAll
        public IEnumerable<PizzaShop.Repository.Models.Section> GetAll()
        {
            return _context.Sections.Where(s => !s.Isdeleted ?? false).ToList();
        }
        #endregion

        #region GetAllSort
        public IEnumerable<PizzaShop.Repository.Models.Section> GetAllSort()
        {
            return _context.Sections
                .Where(s => !s.Isdeleted ?? false)
                .OrderBy(c => c.Sectionorder)
                .ToList();
        }
        #endregion

        #region AddAsync
        public async Task AddAsync(Section section)
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Database context (_context) is null.");
            }

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section), "Section object is null.");
            }
            await _context.Sections.AddAsync(section);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetSectionByIdAsync
        public async Task<PizzaShop.Repository.Models.Section?> GetSectionByIdAsync(int sectionId)
        {
            return await _context.Sections.FindAsync(sectionId);
        }
        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(Section section)
        {
            _context.Sections.Update(section);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region DeleteSection
        public async Task<bool> DeleteSection(int sectionId)
        {
            var section = await _context.Sections.FirstOrDefaultAsync(
                s => s.Sectionid == sectionId
            );
            if (section != null)
            {
                section.Isdeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region SaveAsync
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetAllAsync
        public async Task<IEnumerable<PizzaShop.Repository.Models.Section>> GetAllAsync()
        {
            return await _context.Sections
                .Where(s => !s.Isdeleted ?? false)
                .OrderBy(s => s.Sectionorder)
                .ToListAsync();
        }
        #endregion

        #region CheckDuplicateSectionNameAsync
        public async Task<bool> CheckDuplicateSectionNameAsync(
            string sectionName,
            int? excludeSectionId = null
        )
        {
            return await _context.Sections.AnyAsync(
                s =>
                    s.Sectionname == sectionName
                    && (excludeSectionId == null || s.Sectionid != excludeSectionId)
            );
        }
        #endregion

        #region GetAllSectionsAsync
        public async Task<List<SectionViewModel>> GetAllSectionsAsync()
        {
            var sections = await _context.Sections
                .Where(s => s.Isdeleted == false)
                .OrderBy(s => s.Sectionname)
                .ToListAsync();
            return sections
                .Select(
                    s =>
                        new SectionViewModel
                        {
                            SectionId = s.Sectionid,
                            SectionName = s.Sectionname,
                            Description = s.Description,
                            IsDeleted = s.Isdeleted
                        }
                )
                .ToList();
        }
        #endregion
    }
}
