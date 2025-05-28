using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace PizzaShop.Repository.Models
{
    [Keyless]
    public class WaitingTokenViewModelRawList
    {
        [Column("waiting_token_id")]
        public int WaitingTokenId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("no_of_persons")]
        public int NoOfPersons { get; set; }
    }


}