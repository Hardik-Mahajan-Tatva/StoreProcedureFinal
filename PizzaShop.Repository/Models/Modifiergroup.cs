using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Modifiergroup
{
    public int Modifiergroupid { get; set; }

    public string Modifiergroupname { get; set; } = null!;

    public string? Description { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int Modifiedby { get; set; }

    public int? Sortorder { get; set; }

    public virtual ICollection<Itemmodifiergroupmap> Itemmodifiergroupmaps { get; } = new List<Itemmodifiergroupmap>();

    public virtual ICollection<ModifierGroupModifierMapping> ModifierGroupModifierMappings { get; } = new List<ModifierGroupModifierMapping>();
}
