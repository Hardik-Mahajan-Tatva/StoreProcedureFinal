using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Interfaces
{
    public interface ISectionRepository
    {
        /// <summary>
        /// Retrieves all sections.
        /// </summary>
        /// <returns>A collection of all sections.</returns>
        IEnumerable<PizzaShop.Repository.Models.Section> GetAll();

        /// <summary>
        /// Retrieves all sections sorted by a specific criterion.
        /// </summary>
        /// <returns>A collection of all sorted sections.</returns>
        IEnumerable<PizzaShop.Repository.Models.Section> GetAllSort();

        /// <summary>
        /// Adds a new section asynchronously.
        /// </summary>
        /// <param name="section">The section to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(PizzaShop.Repository.Models.Section section);

        /// <summary>
        /// Retrieves a section by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the section to retrieve.</param>
        /// <returns>A task that returns the section if found, otherwise null.</returns>
        Task<PizzaShop.Repository.Models.Section?> GetSectionByIdAsync(int sectionId);

        /// <summary>
        /// Updates an existing section asynchronously.
        /// </summary>
        /// <param name="section">The section to update.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateAsync(PizzaShop.Repository.Models.Section section);

        /// <summary>
        /// Deletes a section by its ID asynchronously.
        /// </summary>
        /// <param name="sectionId">The ID of the section to delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteSection(int sectionId);

        /// <summary>
        /// Saves changes to the data source asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task SaveAsync();

        /// <summary>
        /// Retrieves all sections asynchronously.
        /// </summary>
        /// <returns>A task that returns a collection of all sections.</returns>
        Task<IEnumerable<PizzaShop.Repository.Models.Section>> GetAllAsync();

        /// <summary>
        /// Checks if a section with the specified name exists, optionally excluding a specific section by ID.
        /// </summary>
        /// <param name="sectionName">The name of the section to check.</param>
        /// <param name="excludeSectionId">The ID of the section to exclude from the check (optional).</param>
        /// <returns>A task that returns true if a duplicate section exists, otherwise false.</returns>
        Task<bool> CheckDuplicateSectionNameAsync(string sectionName, int? excludeSectionId = null);
        Task<List<SectionViewModel>> GetAllSectionsAsync();

        Task<(List<SectionViewModel>, List<WaitingListViewModel>)> GetWaitingListDataAsync(int sectionId);
    }
}
