﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Infrastructure;
using Bazirano.Models.DataAccess;
using Bazirano.Models.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bazirano
{
    public class Startup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration["Data:Bazirano:ConnectionString"]));

            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(
                    Configuration["Data:BaziranoIdentity:ConnectionString"]));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IBoardThreadsRepository, EFRepository>();
            services.AddTransient<IBoardPostsRepository, EFRepository>();
            services.AddTransient<INewsPostsRepository, EFRepository>();
            services.AddTransient<IColumnRepository, EFRepository>();
            services.AddTransient<IColumnRequestsRepository, EFRepository>();
            services.AddTransient<IGoogleRecaptchaHelper, GoogleRecaptchaHelper>();
            services.AddTransient<IWriter, WriterHelper>();

            services.AddMvc();

            var loggingSection = Configuration.GetSection("Logging");
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFile(loggingSection);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } 
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();
            app.UseStatusCodePages();

            SeedData.EnsureCreated(app);
            SeedData.EnsureAdminCreated(app);
        }
    }
}
