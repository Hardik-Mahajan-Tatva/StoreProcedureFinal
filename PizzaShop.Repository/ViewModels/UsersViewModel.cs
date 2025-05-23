using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PizzaShop.Repository.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public required string Firstname { get; set; }

        [Required, StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public required string Lastname { get; set; }

        [Required, StringLength(30, ErrorMessage = "Username cannot exceed 30 characters.")]
        public required string Username { get; set; }

        [Required, EmailAddress, StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public required string Email { get; set; }

        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? Password { get; set; }

        [Required]
        public int Roleid { get; set; }

        [Required]
        public int CountryId { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        public int CityId { get; set; }

        [StringLength(10, ErrorMessage = "Zipcode cannot exceed 10 characters.")]
        public string? Zipcode { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }

        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
        public string? Phone { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        // public string Name => $"{Firstname} {Lastname}";

        public Roles? Roles { get; set; }
    }
    public enum UserStatus
    {
        Active = 1,
        InActive = 0,
    }
}
