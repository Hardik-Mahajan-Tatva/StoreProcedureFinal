using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Interfaces
{
    public interface IWaitingTokenRepository
    {
        /// <summary>
        /// Retrieves all waiting tokens asynchronously.
        /// </summary>
        /// <returns>A task that returns a list of all waiting tokens.</returns>
        Task<List<Waitingtoken>> GetAllWaitingTokenAsync();

        /// <summary>
        /// Retrieves a waiting token by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the waiting token to retrieve.</param>
        /// <returns>A task that returns the waiting token if found, otherwise null.</returns>
        Task<Waitingtoken?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new waiting token asynchronously.
        /// </summary>
        /// <param name="token">The waiting token to add.</param>
        /// <returns>A task that returns true if the addition was successful, otherwise false.</returns>
        Task<bool> AddNewWaitingTokenAsync(Waitingtoken token);

        /// <summary>
        /// Updates an existing waiting token asynchronously.
        /// </summary>
        /// <param name="token">The waiting token to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(Waitingtoken token);

        /// <summary>
        /// Deletes a waiting token by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the waiting token to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(int id);

        /// <summary>
        /// Retrieves a waiting token by customer ID asynchronously.
        /// </summary>
        /// <param name="customerId">The ID of the customer to retrieve the waiting token for.</param>
        /// <returns>A task that returns the waiting token if found, otherwise null.</returns>
        Task<Waitingtoken?> GetWaitingTokenByCustomerIdAsync(int customerId);

        /// <summary>
        /// Updates a waiting token asynchronously.
        /// </summary>
        /// <param name="token">The waiting token to update.</param>
        /// <returns>A task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateWaitingTokenAsync(Waitingtoken token);

        /// <summary>
        /// Deletes a waiting token by its ID asynchronously.
        /// </summary>
        /// <param name="waitingTokenId">The ID of the waiting token to delete.</param>
        /// <returns>A task that returns true if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteWaitingTokenAsync(int waitingTokenId);

        /// <summary>
        /// Retrieves waiting tokens by a list of section IDs asynchronously.
        /// </summary>
        /// <param name="sectionIds">The list of section IDs to retrieve waiting tokens for.</param>
        /// <returns>A task that returns a list of waiting tokens belonging to the specified sections.</returns>
        Task<List<Waitingtoken>> GetTokensBySectionsAsync(List<int> sectionIds);

        /// <summary>
        /// Retrieves the average waiting time within a specified time range asynchronously.
        /// </summary>
        /// <param name="startDate">The start date of the time range.</param>
        /// <param name="endDate">The end date of the time range.</param>
        /// <returns>A task that returns the average waiting time within the time range.</returns>
        Task<decimal> GetAvgWaitingTimeByTimeRangeAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Retrieves the count of today's waiting list asynchronously.
        /// </summary>
        /// <returns>A task that returns the count of today's waiting list.</returns>
        Task<int> GetWaitingListCountOfTodayAsync();

        /// <summary>
        /// Retrieves the count of the waiting list for a specific section asynchronously.
        /// </summary>
        /// <param name="sectionId">The ID of the section to retrieve the waiting list count for.</param>
        /// <returns>A task that returns the count of the waiting list for the specified section.</returns>
        Task<int> GetWaitingListCountBySectionIdAsync(int sectionId);

        /// <summary>
        /// Checks if a mobile number exists in the waiting list asynchronously.
        /// </summary>
        /// <param name="mobileNumber">The mobile number to check.</param>
        /// <returns>A task that returns true if the mobile number exists, otherwise false.</returns>
        Task<bool> IsMobileNumberExistsAsync(string mobileNumber);

        /// <summary>
        /// Checks if an email already exists in the waiting list asynchronously.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>A task that returns true if the email exists, otherwise false.</returns>
        Task<bool> IsEmailAlreadyInWaitingListAsync(string email);

        Task<bool> DeleteWaitingTokenAsyncSP(int waitingTokenId);
        Task<WaitingTokenViewModel?> GetCustomerWaitingDataByEmailAsyncSP(string email);
        Task<bool> AddNewWaitingTokenAsyncSP(WaitingTokenViewModel model);
        Task<TokenWithSectionsResult?> GetWaitingTokenWithSectionsSPAsync(int id);

        Task<bool> UpdateCustomerAndWaitingTokenUsingSPAsync(WaitingTokenViewModel model);
        Task<(bool IsSuccess, string Message, int OrderId)> AssignCustomerToTablesSP(int customerId, List<int> tableIds);
        Task<WaitingTokenViewModelRaw?> GetByIdUsingSPAsync(int customerId);
    }
}
