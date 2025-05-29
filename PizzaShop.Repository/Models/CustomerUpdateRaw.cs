using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PizzaShop.Repository.Models;

[Keyless]
public class CustomerUpdateRaw
{
    [Column("customerid")]
    public int CustomerId { get; set; }

    [Column("customername")]
    public string? CustomerName { get; set; }

    [Column("phoneno")]
    public string? PhoneNumber { get; set; }

    [Column("noofpersons")]
    public short NumberOfPersons { get; set; }

    [Column("email")]
    public string? Email { get; set; }
}
