using FlexiPlace.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    public class CanViewDetailsZahtjevAdminAuthorizationHandler : AuthorizationHandler<DetailsZahtjevRequirement, Zahtjev>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DetailsZahtjevRequirement requirement,
        Zahtjev zahtjev)
        {
            // Ako je admin, može gledati details

            if (context.User.FindFirstValue("Uloga") == "Admin")
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
