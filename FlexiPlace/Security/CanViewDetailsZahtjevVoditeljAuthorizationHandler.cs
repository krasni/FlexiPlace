using FlexiPlace.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    public class CanViewDetailsZahtjevVoditeljAuthorizationHandler : AuthorizationHandler<DetailsZahtjevRequirement, Zahtjev>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DetailsZahtjevRequirement requirement,
        Zahtjev zahtjev)
        {
            // ako je korisnikova uloga "Voditelj" i njegova putanja (claim) je sadržana u putanji korisnika u zahtjevu, može view details

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
