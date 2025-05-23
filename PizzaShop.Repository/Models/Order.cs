using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Order
{
    public int Orderid { get; set; }

    public DateTime? Orderdate { get; set; }

    public int Customerid { get; set; }

    public string? Paymentmode { get; set; }

    public string? Orderwisecomment { get; set; }

    public short? Noofperson { get; set; }

    public decimal? Rating { get; set; }

    public decimal? Subamount { get; set; }

    public decimal? Discount { get; set; }

    public decimal? Totaltax { get; set; }

    public decimal Totalamount { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdby { get; set; }

    public DateTime? Modifiedat { get; set; }

    public int? Modifiedby { get; set; }

    public int Status { get; set; }

    public string? Ordertype { get; set; }

    public string? InvoiceNumber { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Customerreview> Customerreviews { get; } = new List<Customerreview>();

    public virtual ICollection<Invoice> Invoices { get; } = new List<Invoice>();

    public virtual ICollection<Ordereditemmodifer> Ordereditemmodifers { get; } = new List<Ordereditemmodifer>();

    public virtual ICollection<Ordereditem> Ordereditems { get; } = new List<Ordereditem>();

    public virtual ICollection<Ordertable> Ordertables { get; } = new List<Ordertable>();

    public virtual ICollection<Ordertaxmapping> Ordertaxmappings { get; } = new List<Ordertaxmapping>();
}
