using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Taxis
{
    public int Taxid { get; set; }

    public string Taxname { get; set; } = null!;

    public bool? Isenabled { get; set; }

    public bool? Isdefault { get; set; }

    public string Taxtype { get; set; } = null!;

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public float Taxvalue { get; set; }

    public bool? Isinclusive { get; set; }

    public virtual ICollection<Ordertaxmapping> Ordertaxmappings { get; } = new List<Ordertaxmapping>();
}
