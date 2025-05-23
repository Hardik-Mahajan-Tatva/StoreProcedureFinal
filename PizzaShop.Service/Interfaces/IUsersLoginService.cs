using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Service.Interfaces
{
    public interface IUsersLoginService
    {
        /// <summary>
        /// Creates a new user login asynchronously.
        /// </summary>
        /// <param name="login">The view model containing user login details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CreateUserLoginAsync(UsersLoginViewModel login);
    }
}


