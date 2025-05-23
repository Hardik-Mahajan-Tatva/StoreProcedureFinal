using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Ordereditem
{
    public int Ordereditemid { get; set; }

    public int Orderid { get; set; }

    public int Itemid { get; set; }

    public string? Itemwisecomment { get; set; }

    public int Quantity { get; set; }

    public int Readyquantity { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime? Servedat { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<Ordereditemmodifer> Ordereditemmodifers { get; } = new List<Ordereditemmodifer>();
}
