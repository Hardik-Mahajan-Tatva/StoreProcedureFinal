namespace PizzaShop.Repository.ViewModels
{
    public class KOTViewModel
    {
        public int OrderNumber { get; set; }

        public string? ElapsedTime { get; set; }

        public string? CreatedAt { get; set; }

        public DateTime? CreatedAtRaw { get; set; }

        public string? Status { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        public string? SpecialInstructions { get; set; }

        public List<string> SectionNames { get; set; } = new();

        public List<string> TableNames { get; set; } = new();
    }
}