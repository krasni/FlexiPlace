using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using FlexiPlace.Controllers;
using FlexiPlace.Entities;
using FlexiPlace.Models;
using FlexiPlace.Security;
using FlexiPlace.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlexiPlace
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddResponseCompression();

            //services.Configure<GzipCompressionProviderOptions>(options =>
            //{
            //    options.Level = CompressionLevel.Fastest;
            //});

            services.AddDbContextPool<Models.AppDbContext>(
               options => options.UseSqlServer(_config.GetConnectionString("FlexiPlaceDBConnection"), builder => builder.UseRowNumberForPaging()));

            //services.AddDbContextPool<Models.AppDbContext>
            //(
            //   options =>
            //   {
            //       options.UseSqlServer(_config.GetConnectionString("FlexiPlaceDBConnection")).EnableSensitiveDataLogging();
            //   }
            //);

            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            services.AddMvc(options =>
            {
                //var policy = new AuthorizationPolicyBuilder()
                //                     .RequireAuthenticatedUser()
                //                     .Build();
                //options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            services.AddMemoryCache();

            services.AddScoped<IClaimsTransformation, ClaimsTransformer>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "IsAdmin",
                    policy => policy.RequireClaim("IsAdmin"));

                options.AddPolicy(
                    "CanEditZahtjev",
                     policyBuilder => policyBuilder.AddRequirements(new EditZahtjevRequirement()));

                options.AddPolicy(
                    "CanViewDetailsZahtjev",
                     policyBuilder => policyBuilder.AddRequirements(new DetailsZahtjevRequirement()));

                options.AddPolicy(
                    "CanDeleteZahtjev",
                    policyBuilder => policyBuilder.AddRequirements(new DeleteZahtjevRequirement()));

                options.AddPolicy(
                    "CanCancelZahtjev",
                    policyBuilder => policyBuilder.AddRequirements(new CancelZahtjevRequirement()));
                });

            services.AddSingleton<IAuthorizationHandler, CanEditZahtjevVoditeljAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanEditZahtjevAdminAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler, CanViewDetailsZahtjevAdminAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanViewDetailsZahtjevVoditeljAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanViewDetailsZahtjevKorisnikAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler, CanDeleteZahtjevStatusZaOdobravanjeAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanDeleteZahtjevAdminAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanDeleteZahtjevVoditeljAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanDeleteZahtjevKorisnikAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler, CanCancelZahtjevStatusOdobrenAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanCancelZahtjevAdminAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanCancelZahtjevVoditeljAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanCancelZahtjevKorisnikAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, CanCancelZahtjevOdobrenDatumOdsustvaProsaoAuthorizationHandler>();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Zahtjev/AccessDenied");
                options.Cookie.Name = "FlexiPlaceCookie";
            });

            services.Configure<SmtpSettings>(_config.GetSection("SmtpSettings"));
            services.AddSingleton<IMailer, Mailer>();

            services.AddSingleton<INeobradjeniZahtjeviJob, NeobradjeniZahtjeviJob>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure
        (
            IApplicationBuilder app, 
            IHostingEnvironment env
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: null,
                    template: "{statusNaziv}/Strana{zahtjevStrana:int}",
                    defaults: new { controller = "Zahtjev", action = "Index" }
                );

                routes.MapRoute(
                    name: null,
                    template: "Strana{zahtjevStrana:int}",
                    defaults: new
                    {
                        controller = "Zahtjev",
                        action = "Index",
                        zahtjevStrana = 1
                    }
                );

                routes.MapRoute(
                    name: "StatusNaziv",
                    template: "{statusNaziv}",
                    defaults: new
                    {
                        controller = "Zahtjev",
                        action = "Index",
                        zahtjevStrana = 1
                    }
                );

                routes.MapRoute(
                    name: null,
                    template: "",
                    defaults: new
                    {
                        controller = "Zahtjev",
                        action = "Index",
                        zahtjevStrana = 1
                    }
                 );

                routes.MapRoute(name: null, template: "{controller}/{action}/{id?}");
            });
        }
    }
}
