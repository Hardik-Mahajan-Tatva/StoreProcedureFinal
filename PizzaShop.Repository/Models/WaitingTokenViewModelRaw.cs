namespace PizzaShop.Repository.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class WaitingTokenViewModelRaw
    {
        [Column("waiting_token_id")]
        public int WaitingTokenId { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("mobile_number")]
        public string? MobileNumber { get; set; }

        [Column("no_of_persons")]
        public int NoOfPersons { get; set; }

        [Column("section_id")]
        public int SectionId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }
    }


}