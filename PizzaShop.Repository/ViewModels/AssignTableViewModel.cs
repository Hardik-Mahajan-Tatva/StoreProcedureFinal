namespace PizzaShop.Repository.ViewModels
{
    public class AssignTableViewModel
    {
        public List<SectionViewModel>? Sections { get; set; }
        public List<int> SelectedSectionIds { get; set; } = new();
        public int? CustomerId { get; set; }
        public List<int>? TableIds { get; set; }
    }
}