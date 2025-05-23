namespace PizzaShop.Repository.ViewModels
{
    public class WaitingListPageViewModel
    {
        public List<SectionViewModel>? Sections { get; set; }

        public int SelectedSectionId { get; set; }
    }
}