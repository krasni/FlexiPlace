using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlexiPlace.Extensions
{
    public static class Extensions
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {   
            dbSet.RemoveRange(dbSet);
        }

        public static string GetSpecificClaim(this ClaimsIdentity claimsIdentity, string claimType)
        {
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);

            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string PathAndQuery(this HttpRequest request) =>
           request.QueryString.HasValue
               ? $"{request.Path}{request.QueryString}"
               : request.Path.ToString();
    }
}
