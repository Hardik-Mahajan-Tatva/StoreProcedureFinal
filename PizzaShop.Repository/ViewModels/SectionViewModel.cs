using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ViewModels
{
    public class SectionViewModel
    {
        public int SectionId { get; set; }

        [Required(ErrorMessage = "Section name is required.")]
        // [RegularExpression(@"^[A-Za-z][A-Za-z0-9 ]{2,14}$", ErrorMessage = "SectionName must start with a letter and be 3-15 characters long, using only letters, numbers, and spaces.")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9 ]{0,18}[a-zA-Z0-9]$", ErrorMessage = "Section name must start with a letter and be 2-20 characters long, using only letters, numbers, and spaces.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Section name must be between 2 and 20 characters.")]

        public string SectionName { get; set; } = null!;

        [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters.")]
        // [RegularExpression(@"^[A-Za-z0-9\s,.]*$", ErrorMessage = "Description must start with an uppercase letter and can contain letters, numbers, spaces, commas, and periods.")]
        public string? Description { get; set; }

        public bool? IsDeleted { get; set; }

        public int? WaitingListCount { get; set; }
    }
}


