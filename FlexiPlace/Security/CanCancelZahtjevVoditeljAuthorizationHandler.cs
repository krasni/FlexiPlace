using FlexiPlace.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    public class CanCancelZahtjevVoditeljAuthorizationHandler : AuthorizationHandler<CancelZahtjevRequirement, Zahtjev>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CancelZahtjevRequirement requirement, Zahtjev zahtjev)
        {
            if (context.User.FindFirstValue("Uloga") == "Voditelj")
            {
                if (zahtjev.OrganizacijskaJedinicaPutanjaPodnositelj.StartsWith(context.User.FindFirstValue("Putanja")))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.FromResult(0);
        }
    }
}
