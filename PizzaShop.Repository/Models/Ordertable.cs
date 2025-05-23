using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Ordertable
{
    public int Ordertableid { get; set; }

    public int Orderid { get; set; }

    public int Tableid { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
