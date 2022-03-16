using FlexiPlace.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    public class CanDeleteZahtjevStatusZaOdobravanjeAuthorizationHandler : AuthorizationHandler<DeleteZahtjevRequirement, Zahtjev>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeleteZahtjevRequirement requirement, Zahtjev zahtjev)
        {
            if (zahtjev.Status.Naziv != "Za odobravanje")
            {
                context.Fail();
            }

            return Task.FromResult(0);
        }
    }
}
