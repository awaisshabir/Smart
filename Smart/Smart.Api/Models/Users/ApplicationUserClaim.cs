using Microsoft.AspNetCore.Identity;
using System;

namespace Smart.Api.Models.Users
{
    public class ApplicationUserClaim : IdentityUserClaim<Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
