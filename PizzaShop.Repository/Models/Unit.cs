using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Unit
{
    public int Unitid { get; set; }

    public string Unitname { get; set; } = null!;

    public string? Shortname { get; set; }

    public virtual ICollection<Item> Items { get; } = new List<Item>();

    public virtual ICollection<Modifier> Modifiers { get; } = new List<Modifier>();
}
