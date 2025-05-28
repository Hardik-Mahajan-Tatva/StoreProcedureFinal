using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class WaitingTokenService : IWaitingTokenService
{
    private readonly IWaitingTokenRepository _waitingTokenRepo;
    private readonly ICustomerService _customerService;

    private readonly ICustomerRepository _customerRepo;
    private readonly ISectionRepository _sectionRepo;

    public WaitingTokenService(IWaitingTokenRepository waitingTokenRepo, ICustomerService customerService,
        ICustomerRepository customerRepository
        , ISectionRepository sectionRepository)

    {
        _waitingTokenRepo = waitingTokenRepo;
        _customerService = customerService;
        _customerRepo = customerRepository;
        _sectionRepo = sectionRepository;
    }
    public async Task<List<WaitingListViewModel>> GetWaitingListBySectionAsync(int sectionId)
    {
        var allTokens = await _waitingTokenRepo.GetAllWaitingTokenAsync();
        var allSections = await _sectionRepo.GetAllAsync();

        var sectionItemCounts = allSections.ToDictionary(
            section => section.Sectionid,
            section => allTokens.Count(t => t.Sectionid == section.Sectionid)
        );

        var filtered = sectionId == 0
            ? allTokens
            : allTokens.Where(t => t.Sectionid == sectionId).ToList();

        return filtered.Select(t =>
        {
            var customerName = t.Customer?.Customername ?? "Unknown";
            var phone = t.Customer?.Phoneno ?? "N/A";
            var email = t.Customer?.Email ?? "N/A";
            var customerId = t.Customer?.Customerid ?? 0;
            var indiaZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            int itemCountForSection = sectionItemCounts.TryGetValue(t.Sectionid, out int value) ? value : 0;

            return new WaitingListViewModel
            {

                WaitingTokenId = t.Waitingtokenid,
                TokenNo = $"#{t.Waitingtokenid}",


                CreatedAt = t.Createdat.HasValue
    ? TimeZoneInfo.ConvertTimeFromUtc(
          DateTime.SpecifyKind(t.Createdat.Value, DateTimeKind.Utc),
          indiaZone)
    : DateTime.MinValue,



                WaitingTime = t.Createdat.HasValue ? CalculateWaitingTime((DateTime)t.Createdat) : string.Empty,
                Name = customerName,
                Persons = t.Noofpeople,
                Phone = phone,
                Email = email,
                CustomerId = customerId,
                ItemCount = itemCountForSection,
                SectionId = t.Sectionid,
            };
        }).ToList();
    }
    public async Task<List<WaitingListViewModel>> GetWaitingListBySectionAsyncSP(int sectionId)
    {
        var (_, waitingList) = await _sectionRepo.GetWaitingListDataAsync(sectionId);

        // return waitingList;
        return waitingList.Select(t =>
        {
            var customerName = t.Name ?? string.Empty;
            var phone = t.Phone ?? string.Empty;
            var email = t.Email ?? string.Empty;
            var customerId = t.CustomerId ?? 0;
            var indiaZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            // int itemCountForSection = sectionItemCounts.TryGetValue(t.Sectionid, out int value) ? value : 0;
            return new WaitingListViewModel
            {

                WaitingTokenId = t.WaitingTokenId,
                TokenNo = $"#{t.WaitingTokenId}",


                CreatedAt = t.CreatedAt,
                WaitingTime = CalculateWaitingTime(t.CreatedAt),
                Name = customerName,
                Persons = t.Persons,
                Phone = phone,
                Email = email,
                CustomerId = customerId,
                // ItemCount = ,
                SectionId = t.SectionId,
            };
        }).ToList();
    }

    private static string CalculateWaitingTime(DateTime createdAt)
    {
        var indiaZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        var istCreatedAt = TimeZoneInfo.ConvertTime(
            DateTime.SpecifyKind(createdAt, DateTimeKind.Unspecified), indiaZone);

        var istNow = TimeZoneInfo.ConvertTime(
            DateTime.UtcNow, indiaZone);

        var diff = istCreatedAt - istNow;

        if (diff.TotalMinutes < 0)
            return "Just now";

        return $"{(int)diff.TotalHours} hrs {diff.Minutes} min";
    }







    public async Task<bool> AddNewWaitingTokenAsync(WaitingTokenViewModel model)
    {
        if (await _waitingTokenRepo.IsEmailAlreadyInWaitingListAsync(model.Email ?? string.Empty))
        {
            throw new Exception("Customer Already waiting ");

        }
        var existingCustomer = await _customerRepo.GetCustomerByNamePhoneAndEmailAsync(model.Email!);

        int customerId;

        if (existingCustomer != null)
        {

            existingCustomer.Customername = model.Name!;
            existingCustomer.Email = model.Email;
            existingCustomer.Phoneno = model.MobileNumber!;
            existingCustomer.Visitcount = (short?)((existingCustomer.Visitcount ?? 0) + 1);
            existingCustomer.Customerid = existingCustomer.Customerid;

            await _customerRepo.UpdateCustomerAsync(existingCustomer);
            customerId = existingCustomer.Customerid;
        }
        else
        {

            var newCustomer = new Customer
            {
                Customername = model.Name!,
                Phoneno = model.MobileNumber!,
                Email = model.Email,
                Visitcount = 1,
            };

            customerId = await _customerRepo.AddCustomerAsync(newCustomer);
        }


        var newWaitingToken = new Waitingtoken
        {
            Customerid = customerId,
            Noofpeople = model.NoOfPersons,
            Sectionid = model.SectionId ?? 0,
            Isassigned = model.IsAssigned,
        };

        return await _waitingTokenRepo.AddNewWaitingTokenAsync(newWaitingToken);
    }

    public async Task<WaitingTokenViewModel> GetWaitingTokenByIdAsync(int id)
    {
        var token = await _waitingTokenRepo.GetByIdAsync(id);
        var sections = await _sectionRepo.GetAllSectionsAsync();
        if (token == null)
            return new WaitingTokenViewModel();

        return new WaitingTokenViewModel
        {
            WaitingTokenId = token.Waitingtokenid,
            Email = token.Customer?.Email,
            Name = token.Customer?.Customername,
            MobileNumber = token.Customer?.Phoneno,
            NoOfPersons = token.Noofpeople,
            SectionId = token.Sectionid,
            Sections = sections,
            CustomerId = token.Customerid
        };
    }
    public async Task<bool> UpdateWaitingTokenAsync(WaitingTokenViewModel model)
    {

        var existingToken = await _waitingTokenRepo.GetWaitingTokenByCustomerIdAsync(model.CustomerId ?? 0);
        if (existingToken == null)
            return false;


        var customer = await _customerRepo.GetCustomerByCustomerIdAsync(model.CustomerId ?? 0);
        if (customer == null)
            return false;

        customer.Customername = model.Name!;
        customer.Email = model.Email;
        customer.Phoneno = model.MobileNumber!;

        await _customerRepo.UpdateCustomerAsync(customer);

        existingToken.Noofpeople = model.NoOfPersons;
        existingToken.Sectionid = model.SectionId ?? 0;

        return await _waitingTokenRepo.UpdateWaitingTokenAsync(existingToken);
    }

    public async Task<bool> DeleteWaitingTokenAsync(int waitingTokenId)
    {
        return await _waitingTokenRepo.DeleteWaitingTokenAsyncSP(waitingTokenId);
    }

    public async Task<List<WaitingTokenViewModel>> GetWaitingTokensBySectionsAsync(List<int> sectionIds)
    {
        var tokens = await _waitingTokenRepo.GetTokensBySectionsAsync(sectionIds);

        return tokens.Select(t => new WaitingTokenViewModel
        {
            WaitingTokenId = t.Waitingtokenid,
            CustomerId = t.Customerid,
            Name = t.Customer.Customername,
            NoOfPersons = t.Noofpeople
        }).ToList();
    }

    public async Task<WaitingTokenViewModel> GetCustomerDetailsByIdAsync(int customerId)
    {
        var token = await _waitingTokenRepo.GetByIdAsync(customerId);
        var sections = await _sectionRepo.GetAllSectionsAsync();
        if (token == null)
            return new WaitingTokenViewModel();

        return new WaitingTokenViewModel
        {

            WaitingTokenId = token.Waitingtokenid,
            Email = token.Customer?.Email,
            Name = token.Customer?.Customername,
            MobileNumber = token.Customer?.Phoneno,
            NoOfPersons = token.Noofpeople,
            SectionId = token.Sectionid,
            Sections = sections,
            CustomerId = token.Customerid
        };
    }
    public async Task<Waitingtoken?> GetWaitingTokenByCustomerIdAsync(int customerId)
    {
        return await _waitingTokenRepo.GetWaitingTokenByCustomerIdAsync(customerId);
    }
    public async Task<bool> UpdateWaitingTokenStatusAsync(int customerId, bool status)
    {
        var waitingToken = await _waitingTokenRepo.GetWaitingTokenByCustomerIdAsync(customerId);

        if (waitingToken == null)
            return false;

        waitingToken.Isassigned = status;

        await _waitingTokenRepo.UpdateWaitingTokenAsync(waitingToken);

        return true;
    }
    public async Task<bool> IsMobileNumberExistsAsync(string mobileNumber)
    {
        return await _waitingTokenRepo.IsMobileNumberExistsAsync(mobileNumber);
    }


    public async Task<WaitingTokenViewModel?> GetCustomerWithLatestOrderAsync(string email)
    {
        return await _waitingTokenRepo.GetCustomerWaitingDataByEmailAsyncSP(email);
    }

    public async Task<bool> AddNewWaitingTokenAsyncSP(WaitingTokenViewModel model)
    {
        return await _waitingTokenRepo.AddNewWaitingTokenAsyncSP(model);
    }
    public async Task<WaitingTokenViewModel> GetWaitingTokenByIdAsyncsp(int customerId)
    {
        var result = await _waitingTokenRepo.GetWaitingTokenWithSectionsSPAsync(customerId);

        if (result == null || result.Token == null)
            return null!;

        var vm = new WaitingTokenViewModel
        {
            WaitingTokenId = result.Token.WaitingTokenId,
            CustomerId = result.Token.CustomerId,
            Name = result.Token.Name,
            Email = result.Token.Email,
            MobileNumber = result.Token.MobileNumber,
            NoOfPersons = result.Token.NoOfPersons,
            SectionId = result.Token.SectionId,
            Sections = result.Sections?.Select(s => new SectionViewModel
            {
                SectionId = s.SectionId,
                SectionName = s.SectionName
            }).ToList() ?? new List<SectionViewModel>()
        };

        return vm;
    }
    public async Task<bool> UpdateWaitingTokenAsyncSP(WaitingTokenViewModel model)
    {
        return await _waitingTokenRepo.UpdateCustomerAndWaitingTokenUsingSPAsync(model);
    }


}
