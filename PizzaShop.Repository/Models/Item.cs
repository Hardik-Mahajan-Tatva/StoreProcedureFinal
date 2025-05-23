using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Item
{
    public int Itemid { get; set; }

    public string Itemname { get; set; } = null!;

    public int Categoryid { get; set; }

    public decimal Rate { get; set; }

    public int? Quantity { get; set; }

    public int Unitid { get; set; }

    public bool? Isavailable { get; set; }

    public decimal? Taxpercentage { get; set; }

    public string? Shortcode { get; set; }

    public bool? Isfavourite { get; set; }

    public bool? Isdefaulttax { get; set; }

    public string? Itemimg { get; set; }

    public string? Description { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int Modifiedby { get; set; }

    public string Itemtype { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Itemmodifiergroupmap> Itemmodifiergroupmaps { get; } = new List<Itemmodifiergroupmap>();

    public virtual ICollection<Ordereditem> Ordereditems { get; } = new List<Ordereditem>();

    public virtual Unit Unit { get; set; } = null!;
}
