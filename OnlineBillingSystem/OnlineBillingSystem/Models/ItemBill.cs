using System;
using System.Collections.Generic;

namespace OnlineBillingSystem.Models;

public partial class ItemBill
{
    public int ItemId { get; set; }

    public string? ItemName { get; set; }

    public double? UnitPrice { get; set; }

    public double? Quantity { get; set; }

    public double? Discount { get; set; }

    public double? Total { get; set; }

    public int? CustomerId { get; set; }

    public string? Ordered { get; set; }

    public virtual CustomerDetail? Customer { get; set; }
}
