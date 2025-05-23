using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Invoice
{
    public int Invoiceid { get; set; }

    public int Orderid { get; set; }

    public DateTime? Invoicedate { get; set; }

    public string Invoicenumber { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
