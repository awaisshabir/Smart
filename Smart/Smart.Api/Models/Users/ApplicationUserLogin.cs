using Microsoft.AspNetCore.Identity;
using System;

namespace Smart.Api.Models.Users
{
    public class ApplicationUserLogin : IdentityUserLogin<Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
