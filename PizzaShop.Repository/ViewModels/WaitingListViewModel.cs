namespace PizzaShop.Repository.ViewModels
{
    public class WaitingListViewModel
    {
        public string? TokenNo { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? WaitingTime { get; set; }

        public string? Name { get; set; }

        public int Persons { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public int? WaitingTokenId { get; set; }

        public int? CustomerId { get; set; }

        public int? ItemCount { get; set; }

        public int? SectionId { get; set; }
    }
}