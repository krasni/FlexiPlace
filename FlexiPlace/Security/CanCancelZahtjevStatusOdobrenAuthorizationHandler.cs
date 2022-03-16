using FlexiPlace.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    public class CanCancelZahtjevStatusOdobrenAuthorizationHandler : AuthorizationHandler<CancelZahtjevRequirement, Zahtjev>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CancelZahtjevRequirement requirement, Zahtjev zahtjev)
        {
            if (zahtjev.Status.Naziv != "Odobren")
            {
                context.Fail();
            }

            return Task.FromResult(0);
        }
    }
}
