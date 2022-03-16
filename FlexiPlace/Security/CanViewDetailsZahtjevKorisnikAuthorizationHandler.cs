using FlexiPlace.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    public class CanViewDetailsZahtjevKorisnikAuthorizationHandler : AuthorizationHandler<DetailsZahtjevRequirement, Zahtjev>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DetailsZahtjevRequirement requirement,
        Zahtjev zahtjev)
        {
            // ako je korisnikova uloga "Korisnik"  i user je podnositelj, može view details

            if (context.User.FindFirstValue("Uloga") == "Korisnik")
            {
                if (zahtjev.Podnositelj == context.User.FindFirstValue("Username"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.FromResult(0);
        }
    }
}
