using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ViewModels
{
  public class ItemViewModel
  {
    public int Itemid { get; set; }

    [Required(ErrorMessage = "Item Name is required.")]
    // [RegularExpression(@"^[A-Za-z][A-Za-z0-9 ]{2,39}$", ErrorMessage = "Item Name must start with a letter and be 3-40 characters long, using only letters, numbers, and spaces.")]
    // [StringLength(30, MinimumLength = 3, ErrorMessage = "Item Name must be between 3 and 40 characters.")]
    public string Itemname { get; set; } = null!;

    [Required(ErrorMessage = "Category is required.")]
    public int Categoryid { get; set; }

    [Required(ErrorMessage = "Rate is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be a positive number.")]
    public decimal Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
    public int? Quantity { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    public int Unitid { get; set; }

    public bool Isavailable { get; set; }

    [Range(0, 100, ErrorMessage = "Tax percentage must be between 0 and 100.")]
    public decimal? Taxpercentage { get; set; }

    [StringLength(10, ErrorMessage = "Shortcode cannot exceed 10 characters.")]
    public string? Shortcode { get; set; }

    public bool? Isfavourite { get; set; }

    public bool Isdefaulttax { get; set; }

    public string? Itemimg { get; set; }

    [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters.")]
    //  [RegularExpression(@"^[A-Z][A-Za-z0-9\s,.]*$", ErrorMessage = "Description must start with an uppercase letter and can contain letters, numbers, spaces, commas, and periods.")]
    public string? Description { get; set; }

    public bool? Isdeleted { get; set; }

    public int? ModifierId { get; set; }

    [Required(ErrorMessage = "Item Type is required.")]
    public string? ItemType { get; set; }

    public IEnumerable<ModifiergroupViewModel>? Modifiergroups { get; set; }

    public IEnumerable<CategoryViewModel>? Categories { get; set; }

    public IEnumerable<UnitViewModel>? Units { get; set; }

    public List<KeyValuePair<int, string>>? ItemTypes { get; set; }
  }
}
