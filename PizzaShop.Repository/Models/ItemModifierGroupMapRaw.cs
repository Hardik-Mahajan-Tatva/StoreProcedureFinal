using PizzaShop.Repository.ViewModels;

namespace PizzaShop.Repository.Models;


public class ItemModifierGroupMapRaw
{
    public int ItemId { get; set; }
    public int ModifierGroupId { get; set; }
    public string? ModifierGroupName { get; set; }
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }

    public string ModifierItems { get; set; } = string.Empty;
}

