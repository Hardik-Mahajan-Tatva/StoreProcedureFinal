using System.ComponentModel.DataAnnotations;
namespace PizzaShop.Repository.ViewModels
{
    public class SpecialInstructionViewModel
    {
        public int OrderId { get; set; }

        public int OrderedItemId { get; set; }

        [StringLength(100, MinimumLength = 10, ErrorMessage = "Instruction must be between 10 and 100 characters.")]
        // [RegularExpression(@"^[A-Z][A-Za-z0-9\s,.]*$", ErrorMessage = "Description must start with an uppercase letter and can contain letters, numbers, spaces, commas, and periods.")]
        public string? Instruction { get; set; }
    }
}