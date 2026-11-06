using System;
using System.Collections.Generic;

namespace PeopleAccounts.web.Models;

public partial class Status
{
    public int Code { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
