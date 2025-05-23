using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class UsersLoginService : IUsersLoginService
{
    private readonly IUsersloginRepository _usersLoginRepository;

    public UsersLoginService(IUsersloginRepository usersLoginRepository)
    {
        _usersLoginRepository = usersLoginRepository;
    }

    public async Task CreateUserLoginAsync(UsersLoginViewModel viewModel)
    {
        try
        {
            var loginEntity = new Userslogin
            {
                Username = viewModel.Username,
                Passwordhash = viewModel.Passwordhash,
                Email = viewModel.Email,
                Userid = viewModel.Userid,
                Roleid = viewModel.Roleid,
                Isfirstlogin = true
            };

            await _usersLoginRepository.CreateUserLoginAsync(loginEntity);
        }
        catch (Exception ex)
        {
            var error = ex.InnerException?.Message ?? ex.Message;
            throw new Exception("An error occurred while creating the user login. Details: " + error, ex);
        }
    }
}

