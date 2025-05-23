using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Ordereditemmodifer
{
    public int Modifieditemid { get; set; }

    public int Ordereditemid { get; set; }

    public int Quantity { get; set; }

    public int Orderid { get; set; }

    public int Itemmodifierid { get; set; }

    public virtual Modifier Itemmodifier { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual Ordereditem Ordereditem { get; set; } = null!;
}
