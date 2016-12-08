﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stormpath.AspNetCore;
using WebApiAngularStorm.Models;
using stormpath_angularjs_dotnet_stripe_twilio.Models;
using Stripe;
using stormpath_angularjs_dotnet_stripe_twilio.Services;
using Stormpath.Configuration.Abstractions;
using System.IO;

namespace stormpath_angularjs_dotnet_stripe_twilio
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
                if (env.IsDevelopment())
                {
                    builder.AddUserSecrets();
                }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStormpath(new StormpathOptions() {
                PostRegistrationHandler = async (ctx, ct) =>
                {
                    ctx.Account.CustomData["Balance"] = 0;
                    ctx.Account.CustomData["TotalQueries"] = 0;
                    await ctx.Account.SaveAsync();
                    await ctx.Account.CreateApiKeyAsync();
                },
                
            }
            );
            services.AddTransient<StripeChargeService>();
            services.AddTransient<PaymentService>();
            services.AddTransient<AccountService>();
            services.AddTransient<SMSService>();
            services.AddTransient<BitcoinExchangerRateService>();
            services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase());
            services.Configure<SMSSettings>(Configuration.GetSection("SMSSettings"));
            services.Configure<PaymentSettings>(Configuration.GetSection("PaymentSettings"));
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseStormpath();
            var context = app.ApplicationServices.GetService<ApiContext>();
            SeedDatabase(context);            

            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");

            // This redirects all client-side routes to the index, keeping the URL path
            // intact for deep linking
            app.Use(async (c, next) =>
            {
                await next();

                if (c.Response.StatusCode == 404 && !Path.HasExtension(c.Request.Path.Value))
                {
                    c.Request.Path = "/index.html";
                    await next();
                }
            })
            .UseCors("AllowAll")
            .UseMvc()
            .UseDefaultFiles(options)
            .UseStaticFiles();
        }

        private static void SeedDatabase(ApiContext context)
        {
            context.Todos.Add(new Todo
            {
                Id = 1,
                Description = "My First Todo",
                Completed = false,
                User = "me@mymail.com"
            });

            context.Todos.Add(new Todo
            {
                Id = 2,
                Description = "My Second Todo",
                Completed = false,
                User = "me@mymail.com"
            });

            context.SaveChanges();
        }
    }
}