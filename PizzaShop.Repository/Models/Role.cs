using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Role
{
    public int Roleid { get; set; }

    public string Rolename { get; set; } = null!;

    public virtual ICollection<Permission> Permissions { get; } = new List<Permission>();

    public virtual ICollection<Userslogin> Userslogins { get; } = new List<Userslogin>();
}
