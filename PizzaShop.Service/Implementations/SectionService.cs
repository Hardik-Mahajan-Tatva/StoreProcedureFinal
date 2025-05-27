using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;
namespace PizzaShop.Service.Implementations
{
    public class SectionService : ISectionService
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly IWaitingTokenRepository _waitingTokenRepository;

        public SectionService(ISectionRepository sectionRepository,
        IWaitingTokenRepository waitingTokenRepository
        )
        {
            _waitingTokenRepository = waitingTokenRepository;
            _sectionRepository = sectionRepository;
        }

        public IEnumerable<SectionViewModel> GetAll()
        {
            var sections = _sectionRepository.GetAllSort();
            return sections.Select(s => new SectionViewModel
            {
                SectionId = s.Sectionid,
                SectionName = s.Sectionname,
                Description = s.Description,
                IsDeleted = s.Isdeleted
            });
        }
        public async Task AddAsync(SectionViewModel sectionViewModel)
        {
            Section section = new()
            {
                Sectionname = sectionViewModel.SectionName,
                Description = sectionViewModel.Description
            };

            await _sectionRepository.AddAsync(section);
        }

        public async Task<SectionViewModel> GetSectionByIdAsync(int id)
        {
            var section = await _sectionRepository.GetSectionByIdAsync(id);

            if (section == null)
            {
                return new SectionViewModel();
            }

            return new SectionViewModel
            {
                SectionId = section.Sectionid,
                SectionName = section.Sectionname,
                Description = section.Description
            };
        }

        public async Task<bool> UpdateAsync(SectionViewModel section)
        {
            var entity = await _sectionRepository.GetSectionByIdAsync(section.SectionId);
            if (entity == null) return false;

            bool isDuplicate = await _sectionRepository.CheckDuplicateSectionNameAsync(section.SectionName, section.SectionId);
            if (isDuplicate)
            {
                throw new InvalidOperationException("A section with the same name already exists.");
            }
            entity.Sectionname = section.SectionName;
            entity.Description = section.Description;

            return await _sectionRepository.UpdateAsync(entity);
        }
        public async Task<bool> DeleteSection(int sectionId)
        {
            if (sectionId == 0)
            {
                return false;
            }

            var deleted = await _sectionRepository.DeleteSection(sectionId);
            return deleted;
        }
        public async Task UpdateSectionOrderAsync(List<int> sortedSectionIds)
        {
            var sections = _sectionRepository.GetAllSort();

            foreach (var (sectionId, index) in sortedSectionIds.Select((id, idx) => (id, idx)))
            {
                var section = sections.FirstOrDefault(s => s.Sectionid == sectionId);
                if (section != null)
                {
                    section.Sectionorder = index + 1;
                }
            }

            await _sectionRepository.SaveAsync();
        }
        public async Task<List<SectionViewModel>> GetAllSectionsAsyncSP()
        {
            var (sections, _) = await _sectionRepository.GetWaitingListDataAsync(0);
            return sections;
        }
        public async Task<List<SectionViewModel>> GetAllSectionsAsync()
        {
            var sections = await _sectionRepository.GetAllAsync();

            var sectionViewModels = new List<SectionViewModel>();

            foreach (var section in sections)
            {
                // Get the waiting list count for this section
                var waitingListCount = await _waitingTokenRepository.GetWaitingListCountBySectionIdAsync(section.Sectionid);

                sectionViewModels.Add(new SectionViewModel
                {
                    SectionId = section.Sectionid,
                    SectionName = section.Sectionname,
                    Description = section.Description,
                    WaitingListCount = waitingListCount
                });
            }

            return sectionViewModels;
        }

        public async Task<bool> CheckDuplicateSectionNameAsync(string sectionName, int? excludeSectionId = null)
        {
            return await _sectionRepository.CheckDuplicateSectionNameAsync(sectionName, excludeSectionId);
        }


    }
}

