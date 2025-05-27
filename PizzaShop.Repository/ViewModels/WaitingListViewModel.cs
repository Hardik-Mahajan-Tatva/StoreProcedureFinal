using System.Text.Json.Serialization;

namespace PizzaShop.Repository.ViewModels
{
    public class WaitingListViewModel
    {
        public string? TokenNo { get; set; }

        [JsonPropertyName("createdat")]
        public DateTime CreatedAt { get; set; }

        public string? WaitingTime { get; set; }

        [JsonPropertyName("customername")]
        public string? Name { get; set; }

        [JsonPropertyName("noofpeople")]
        public int Persons { get; set; }

        [JsonPropertyName("phoneno")]
        public string? Phone { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("waitingtokenid")]
        public int? WaitingTokenId { get; set; }

        [JsonPropertyName("customerid")]
        public int? CustomerId { get; set; }

        public int? ItemCount { get; set; }

        [JsonPropertyName("sectionid")]
        public int? SectionId { get; set; }
    }
}