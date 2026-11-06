using System;
using System.Collections.Generic;

namespace PeopleAccounts.web.Models;

public partial class Person
{
    public int Code { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string IdNumber { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
