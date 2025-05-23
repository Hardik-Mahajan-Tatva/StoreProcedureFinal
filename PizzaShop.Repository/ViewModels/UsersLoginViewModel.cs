namespace PizzaShop.Repository.ViewModels
{
    public class UsersLoginViewModel
    {
        public string Email { get; set; } = null!;

        public string Passwordhash { get; set; } = null!;

        public int? Userid { get; set; }

        public int Roleid { get; set; }

        public string Username { get; set; } = null!;

        public Status Status { get; set; }
    }
    public enum Status
    {
        Active = 1,
        InActive = 2
    }
}
