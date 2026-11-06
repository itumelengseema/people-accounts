using System;
using System.Collections.Generic;

namespace PeopleAccounts.web.Models;

public partial class Account
{
    public int Code { get; set; }

    public int PersonCode { get; set; }

    public string AccountNumber { get; set; } = null!;

    public decimal OutstandingBalance { get; set; }

    public int StatusCode { get; set; }

    public virtual Person PersonCodeNavigation { get; set; } = null!;

    public virtual Status StatusCodeNavigation { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
