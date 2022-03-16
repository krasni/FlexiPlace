using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.Security
{
    using System.Security.Claims; // for ClaimsPrincipal
    using FlexiPlace.Models;
    using Microsoft.AspNetCore.Authentication; // for IClaimsTransformation
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;

    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly AppDbContext dbContext;
        private readonly IMemoryCache cache;
        private readonly IConfiguration configuration;

        public ClaimsTransformer(AppDbContext dbContext, IMemoryCache cache, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.cache = cache;
            this.configuration = configuration;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var cacheKey = principal.FindFirstValue(ClaimTypes.Name);
                
            if (cache.TryGetValue(cacheKey, out List<Claim> userClaims))
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(userClaims);
            }
            else
            {
                userClaims = new List<Claim>();

                var admins = dbContext.Admin.ToList();

                var ci = (ClaimsIdentity)principal.Identity;
                var claim = ci.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

                var userDetails = dbContext.GetUserDataBP2(claim.Value);

                if (userDetails[0].Uloga == "Admin")
                {
                    userClaims.Add(new Claim("IsAdmin", "yes"));
                }

                userClaims.Add(new Claim("Username", userDetails[0].UserName));
                userClaims.Add(new Claim("ImePrezime", userDetails[0].ImePrezime));
                userClaims.Add(new Claim("Email", userDetails[0].Email));
                userClaims.Add(new Claim("Uloga", userDetails[0].Uloga));
                userClaims.Add(new Claim("UlogaHanfa", userDetails[0].UlogaHanfa));
                userClaims.Add(new Claim("NazivOrganizacijskeJedinice", userDetails[0].NazivOrganizacijskeJedinice));
                userClaims.Add(new Claim("Lvl", userDetails[0].Lvl.ToString()));
                userClaims.Add(new Claim("Putanja", userDetails[0].Putanja));
                userClaims.Add(new Claim("OdobravateljADName", userDetails[0].OdobravateljADName));
                userClaims.Add(new Claim("OdobravateljImePrezime", userDetails[0].OdobravateljImePrezime));
                userClaims.Add(new Claim("OdobravateljNazivOrganizacijskeJedinice", userDetails[0].OdobravateljNazivOrganizacijskeJedinice));
                userClaims.Add(new Claim("OdobravateljPutanja", userDetails[0].OdobravateljPutanja));
                userClaims.Add(new Claim("OdobravateljEmail", userDetails[0].OdobravateljEmail));

                ci.AddClaims(userClaims);

                var refreshPermissionsPeriodInSeconds = int.Parse( configuration.GetSection("RefreshPermissionsPeriodInSeconds").Value);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(refreshPermissionsPeriodInSeconds));

                 cache.Set(cacheKey, userClaims, cacheEntryOptions);
            }

            return Task.FromResult(principal);
        }
    }
}
