using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class Accounting
{
    public int AccountingId { get; set; }

    public string AccountNumber { get; set; } = null!;

    public string AccountName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal DebitAmount { get; set; }

    public decimal CreditAmount { get; set; }

    public DateTime TransactionDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsReconciled { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;
}
