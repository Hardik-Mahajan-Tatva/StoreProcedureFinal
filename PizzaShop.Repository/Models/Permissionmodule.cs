using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Permissionmodule
{
    public int Moduleid { get; set; }

    public string Modulename { get; set; } = null!;

    public virtual ICollection<Permission> Permissions { get; } = new List<Permission>();
}
