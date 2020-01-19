using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DattingApp.API.Model
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}