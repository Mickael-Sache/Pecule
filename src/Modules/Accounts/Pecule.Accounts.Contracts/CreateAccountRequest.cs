using System;
using System.Collections.Generic;
using System.Text;

namespace Pecule.Accounts.Contracts;

public sealed record CreateAccountRequest(string Name, AccountType Type, decimal InitialBalance);
