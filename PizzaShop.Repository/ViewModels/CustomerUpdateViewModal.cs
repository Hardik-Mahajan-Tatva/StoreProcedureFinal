using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ViewModels
{
    public class CustomerUpdateViewModal
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "NumberOfPersons is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "NumberOfPersons must be a positive number.")]
        public int? NumberOfPersons { get; set; }

        [Required(ErrorMessage = "CustomerName is required.")]
        [StringLength(50, ErrorMessage = "CustomerName cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "Name must contain only letters and be between 2 and 50 characters.")]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email Address format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }
    }
}



