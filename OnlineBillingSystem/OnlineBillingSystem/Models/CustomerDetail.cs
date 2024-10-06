using System;
using System.Collections.Generic;

namespace OnlineBillingSystem.Models;

public partial class CustomerDetail
{
    public int CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public string? PaymentMethod { get; set; }

    public double? FinalTotal { get; set; }

    public string? CardNumber { get; set; }

    public virtual ICollection<ItemBill> ItemBills { get; set; } = new List<ItemBill>();
}
