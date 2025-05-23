using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ViewModels
{
    public class CustomerOrderViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email Address format")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "Name must contain only letters and be between 2 and 50 characters.")]
        [Display(Name = "Full Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format.")]
        [Display(Name = "Mobile Number")]
        public string? MobileNumber { get; set; }

        [Required(ErrorMessage = "NoOfPersons is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of Persons must be a positive number.")]
        [Display(Name = "Number of Persons")]
        public int NoOfPersons { get; set; }

        [Display(Name = "Section ID")]
        public int? SectionId { get; set; }

        public List<int> TableIds { get; set; } = new List<int>();

        public int? CustomerId { get; set; }
    }
}