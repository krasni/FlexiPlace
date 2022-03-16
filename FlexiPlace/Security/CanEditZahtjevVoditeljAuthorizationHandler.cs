using FlexiPlace.Extensions;
using FlexiPlace.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    public class CanEditZahtjevVoditeljAuthorizationHandler : AuthorizationHandler<EditZahtjevRequirement, Zahtjev>
    {
        protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        EditZahtjevRequirement requirement,
        Zahtjev zahtjev)
        {
            // voditelj može samo editirati zahtjeve koji su za odobravanje
            // ako je korisnikova uloga "Voditelj" i njegova putanja (claim) je sadržana u putanji korisnika u zahtjevu
            // tada ako ADName u zahtjevu nije jednak ADName trenutnog korisnika (claim), onda može editirati.
            // opaska: Ako je ADName u zahtjevu jednak ADName trenutnog korisnika, to znači da je on napravio zahtjev
            // pa nemože editirati samog sebe

            if (context.User.FindFirstValue("Uloga") == "Voditelj")
            {
                if (zahtjev.OrganizacijskaJedinicaPutanjaPodnositelj.StartsWith(context.User.FindFirstValue("Putanja")) && (zahtjev.Status.Naziv == "Za odobravanje"))
                {
                    if (zahtjev.Podnositelj != context.User.FindFirstValue("Username"))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.FromResult(0);
        }
    }
}
