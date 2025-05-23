using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Customerreview
{
    public int Reviewid { get; set; }

    public int Orderid { get; set; }

    public decimal? Foodrating { get; set; }

    public decimal? Servicerating { get; set; }

    public decimal? Ambiencerating { get; set; }

    public float? Avgrating { get; set; }

    public string? Comments { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Modifiedat { get; set; }

    public virtual Order Order { get; set; } = null!;
}
