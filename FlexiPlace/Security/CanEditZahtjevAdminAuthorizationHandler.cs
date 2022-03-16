using FlexiPlace.Extensions;
using FlexiPlace.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    public class CanEditZahtjevAdminAuthorizationHandler : AuthorizationHandler<EditZahtjevRequirement, Zahtjev>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EditZahtjevRequirement requirement,
        Zahtjev zahtjev)
        {
            // Ako je admin i nije njegov zahtjev, može editirati

            if (context.User.FindFirstValue("Uloga") == "Admin")
            {
                if (zahtjev.Podnositelj != context.User.FindFirstValue("Username"))
                {
                    context.Succeed(requirement);
                }

                //context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
