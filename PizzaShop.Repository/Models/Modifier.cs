using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Modifier
{
    public int Modifierid { get; set; }

    public string Modifiername { get; set; } = null!;

    public decimal Rate { get; set; }

    public int? Quantity { get; set; }

    public int Unitid { get; set; }

    public string? Description { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public int? Modifiertype { get; set; }

    public virtual ICollection<ModifierGroupModifierMapping> ModifierGroupModifierMappings { get; } = new List<ModifierGroupModifierMapping>();

    public virtual ICollection<Ordereditemmodifer> Ordereditemmodifers { get; } = new List<Ordereditemmodifer>();

    public virtual Unit Unit { get; set; } = null!;
}
