using FlexiPlace.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    public class CanCancelZahtjevOdobrenDatumOdsustvaProsaoAuthorizationHandler : AuthorizationHandler<CancelZahtjevRequirement, Zahtjev>
    { 
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CancelZahtjevRequirement requirement, Zahtjev zahtjev)
        {
            if (zahtjev.DatumOdsustva < (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)))
            {
                context.Fail();
            }

            return Task.FromResult(0);
        }
    }
}
