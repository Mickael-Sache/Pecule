using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Pecule.Accounts;

internal sealed class User : IdentityUser<Guid>;