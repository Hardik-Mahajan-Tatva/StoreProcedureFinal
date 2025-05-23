using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.ViewModels
{
    public class UpdateUserViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "Name must contain only letters and be between 2 and 50 characters.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "Last Name must contain only letters and be between 2 and 50 characters.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Username  is required.")]
        [StringLength(20, ErrorMessage = "Username cannot exceed 20 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$", ErrorMessage = "Username can only contain letters, numbers, and underscores, and must be between 3-20 characters.")]
        public required string Username { get; set; }

        [RegularExpression(@"^(Admin|User|Manager)$", ErrorMessage = "Please selece any one role.")]
        public string? Role { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        [Required(ErrorMessage = "Zipcode is required.")]
        [RegularExpression(@"^\d{5,10}$", ErrorMessage = "Invalid Zipcode format.")]
        public string? Zipcode { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format.")]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression(@"^(Active|InActive)$", ErrorMessage = "Please selece any one status.")]
        public required UserStatus Status { get; set; }

        [Required(ErrorMessage = "Please selece a valid country.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid country.")]
        public int Countryid { get; set; }

        [Required(ErrorMessage = "Please selece a valid State.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid state.")]
        public int Stateidid { get; set; }

        [Required(ErrorMessage = "Please selece a valid City.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid city.")]
        public int Cityidid { get; set; }

        public int RoleId { get; set; }

        public IFormFile? ProfileFile { get; set; }

        public string? ProfileImage { get; set; }

        public List<Role>? Roles { get; set; }

        public List<Country>? Countries { get; set; }

        public List<State>? States { get; set; }

        public List<City>? Cities { get; set; }
    }
}
