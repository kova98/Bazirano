using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.Admin
{
    public class AccountsViewModel
    {
        public List<(IdentityUser, IList<string>)> UserRolesPairs { get; set; }

        public List<IdentityRole> Roles { get; set; }
    }
}
