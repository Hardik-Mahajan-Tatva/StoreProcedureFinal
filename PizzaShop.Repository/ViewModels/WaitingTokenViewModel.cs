using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ViewModels
{
    public class WaitingTokenViewModel
    {
        [EmailAddress]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email Address format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]{2,50}$", ErrorMessage = "Name must contain only letters and be between 2 and 50 characters.")]
        [Display(Name = "Full Name")]
        public string? Name { get; set; }

        [Display(Name = "Mobile Number")]
        [Phone]
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format.")]
        public string? MobileNumber { get; set; }

        [Display(Name = "No. of Persons")]
        [Required(ErrorMessage = "NoOfPersons is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "NoOfPersons must be a positive number.")]
        public int NoOfPersons { get; set; }

        [Required(ErrorMessage = "Section is required.")]
        [Display(Name = "Section")]
        public int? SectionId { get; set; }

        public List<SectionViewModel>? Sections { get; set; }

        public int? CustomerId { get; set; }

        public bool IsAssigned { get; set; } = false;

        public int? WaitingTokenId { get; set; }
    }
}