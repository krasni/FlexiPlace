using FlexiPlace.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    public class CanDeleteZahtjevKorisnikAuthorizationHandler : AuthorizationHandler<DeleteZahtjevRequirement, Zahtjev>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeleteZahtjevRequirement requirement, Zahtjev zahtjev)
        {
            if (context.User.FindFirstValue("Uloga") == "Korisnik")
            {
                if (zahtjev.OrganizacijskaJedinicaPutanjaPodnositelj == (context.User.FindFirstValue("Putanja")) && zahtjev.Podnositelj == context.User.FindFirstValue("Username"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.FromResult(0);
        }
    }
}
