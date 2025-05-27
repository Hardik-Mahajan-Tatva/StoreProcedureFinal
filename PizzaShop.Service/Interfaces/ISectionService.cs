using PizzaShop.Repository.ViewModels;
namespace PizzaShop.Service.Interfaces
{
   public interface ISectionService
   {
      /// <summary>
      /// Retrieves all sections.
      /// </summary>
      /// <returns>A collection of section view models.</returns>
      IEnumerable<SectionViewModel> GetAll();

      /// <summary>
      /// Adds a new section asynchronously.
      /// </summary>
      /// <param name="sectionViewModel">The view model containing section details.</param>
      /// <returns>A task representing the asynchronous operation.</returns>
      Task AddAsync(SectionViewModel sectionViewModel);

      /// <summary>
      /// Retrieves a section by its ID asynchronously.
      /// </summary>
      /// <param name="id">The ID of the section to retrieve.</param>
      /// <returns>A task that returns the section view model if found, otherwise null.</returns>
      Task<SectionViewModel> GetSectionByIdAsync(int id);

      /// <summary>
      /// Updates an existing section asynchronously.
      /// </summary>
      /// <param name="section">The view model containing updated section details.</param>
      /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
      Task<bool> UpdateAsync(SectionViewModel section);

      /// <summary>
      /// Deletes a section by its ID asynchronously.
      /// </summary>
      /// <param name="sectionId">The ID of the section to delete.</param>
      /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
      Task<bool> DeleteSection(int sectionId);

      /// <summary>
      /// Updates the order of sections asynchronously.
      /// </summary>
      /// <param name="sortedSectionIds">The list of section IDs in the desired order.</param>
      /// <returns>A task representing the asynchronous operation.</returns>
      Task UpdateSectionOrderAsync(List<int> sortedSectionIds);

      /// <summary>
      /// Retrieves all sections asynchronously.
      /// </summary>
      /// <returns>A task that returns a collection of section view models.</returns>
      Task<List<SectionViewModel>> GetAllSectionsAsync();

      /// <summary>
      /// Checks if a section with the specified name exists, optionally excluding a specific section by ID.
      /// </summary>
      /// <param name="sectionName">The name of the section to check.</param>
      /// <param name="excludeSectionId">The ID of the section to exclude from the check (optional).</param>
      /// <returns>A task that returns true if a duplicate section exists, otherwise false.</returns>
      Task<bool> CheckDuplicateSectionNameAsync(string sectionName, int? excludeSectionId = null);

         Task<List<SectionViewModel>> GetAllSectionsAsyncSP();
   }
}

