using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IWaitingTokenService
{
    Task<bool> AddNewWaitingTokenAsync(WaitingTokenViewModel model);
    Task<List<WaitingListViewModel>> GetWaitingListBySectionAsync(int sectionId);
    Task<WaitingTokenViewModel> GetWaitingTokenByIdAsync(int id);
    Task<bool> UpdateWaitingTokenAsync(WaitingTokenViewModel model);
    Task<bool> DeleteWaitingTokenAsync(int waitingTokenId);
    Task<List<WaitingTokenViewModel>> GetWaitingTokensBySectionsAsync(List<int> sectionIds);
    Task<WaitingTokenViewModel> GetCustomerDetailsByIdAsync(int tokenId);
    Task<Waitingtoken?> GetWaitingTokenByCustomerIdAsync(int customerId);
    Task<bool> UpdateWaitingTokenStatusAsync(int customerId, bool status);
    Task<bool> IsMobileNumberExistsAsync(string mobileNumber);

    Task<List<WaitingListViewModel>> GetWaitingListBySectionAsyncSP(int sectionId);
    Task<WaitingTokenViewModel?> GetCustomerWithLatestOrderAsync(string email);
    Task<bool> AddNewWaitingTokenAsyncSP(WaitingTokenViewModel model);
    Task<WaitingTokenViewModel> GetWaitingTokenByIdAsyncsp(int id);
}
