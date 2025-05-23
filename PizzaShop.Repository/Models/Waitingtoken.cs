using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Waitingtoken
{
    public int Waitingtokenid { get; set; }

    public int Customerid { get; set; }

    public int Noofpeople { get; set; }

    public int Sectionid { get; set; }

    public bool? Isassigned { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Section Section { get; set; } = null!;

    public virtual ICollection<Waitingtablemapping> Waitingtablemappings { get; } = new List<Waitingtablemapping>();
}
