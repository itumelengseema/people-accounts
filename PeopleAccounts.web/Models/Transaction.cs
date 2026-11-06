using System;
using System.Collections.Generic;

namespace PeopleAccounts.web.Models;

public partial class Transaction
{
    public int Code { get; set; }

    public int AccountCode { get; set; }

    public DateTime TransactionDate { get; set; }

    public DateTime CaptureDate { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; } = null!;

    public virtual Account AccountCodeNavigation { get; set; } = null!;
}
