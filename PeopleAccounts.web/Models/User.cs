using System;
using System.Collections.Generic;

namespace PeopleAccounts.web.Models;

public partial class User
{
    public int Code { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
}
