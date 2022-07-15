using Microsoft.AspNetCore.Identity;
using System;

namespace Smart.Api.Models.Users
{
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}
