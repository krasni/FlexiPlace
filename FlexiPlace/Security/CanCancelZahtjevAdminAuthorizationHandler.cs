using FlexiPlace.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    public class CanCancelZahtjevAdminAuthorizationHandler : AuthorizationHandler<CancelZahtjevRequirement, Zahtjev>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CancelZahtjevRequirement requirement, Zahtjev zahtjev)
        {
            if (context.User.FindFirstValue("Uloga") == "Admin")
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
