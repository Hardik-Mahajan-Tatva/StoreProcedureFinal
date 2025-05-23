using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Country
{
    public int Countryid { get; set; }

    public string Shortname { get; set; } = null!;

    public string Countryname { get; set; } = null!;

    public int Phonecode { get; set; }

    public DateTime? Createdat { get; set; }

    public int Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int Modifiedby { get; set; }
}
