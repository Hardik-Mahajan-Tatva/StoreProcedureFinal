using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Orderedmodifier
{
    public int Orderedmodifierid { get; set; }

    public int Ordereditemid { get; set; }

    public int Modifierid { get; set; }

    public int Quantity { get; set; }

    public int Orderid { get; set; }

    public virtual Modifier Modifier { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual Ordereditem Ordereditem { get; set; } = null!;
}
