﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Extensions.Hosting;
using SeekAndDestroy.Core.DataAccess;
using SeekAndDestroy.Infrastructure.DataAccess.Repositories;

namespace SeekAndDestroy.Infrastructure.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            this.ConfigureOpenIdConnect(services);
            this.ConfigureDependencyInjection(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            services.AddTransient<IUserRepository, UserRepository>((provider) => new UserRepository(config["ConnectionString"]));
            services.AddTransient<IBuildingsRepository, BuildingsRepository>((provider) => new BuildingsRepository(config["ConnectionString"]));
            services.AddTransient<IResourcesRepository, ResourcesRepository>((provider) => new ResourcesRepository(config["ConnectionString"]));
        }

        private void ConfigureOpenIdConnect(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect((options) => {
                    options.Scope.Add("email");
                    options.ClientId = Environment.GetEnvironmentVariable("GoogleClientId");
                    options.ClientSecret = Environment.GetEnvironmentVariable("GoogleClientSecret");
                    options.Authority = "https://accounts.google.com";
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                    options.Events = new OpenIdConnectEvents()
                    {
                        OnRedirectToIdentityProvider = (context) =>
                        {
                            if (context.Request.Path != "/account/external")
                            {
                                context.Response.Redirect("/account/login");
                                context.HandleResponse();
                            }

                            return Task.FromResult(0);
                        }
                    };
                }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
