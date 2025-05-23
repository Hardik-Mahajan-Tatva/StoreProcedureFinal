using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Category
{
    public int Categoryid { get; set; }

    public string Categoryname { get; set; } = null!;

    public string? Description { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int Modifiedby { get; set; }

    public int? Sortorder { get; set; }

    public virtual ICollection<Item> Items { get; } = new List<Item>();
}
