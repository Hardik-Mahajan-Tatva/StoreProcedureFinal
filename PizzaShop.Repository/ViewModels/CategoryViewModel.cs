using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9 ]{2,19}$", ErrorMessage = "Category Name must start with a letter and be 3-20 characters long, using only letters, numbers, and spaces.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Category Name must be between 3 and 20 characters.")]
        public string? CategoryName { get; set; }

        [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters.")]
        // [RegularExpression(@"^[A-Z][A-Za-z0-9\s,.]*$", ErrorMessage = "Description must start with an uppercase letter and can contain letters, numbers, spaces, commas, and periods.")]
        public string? Description { get; set; }
    }
}
