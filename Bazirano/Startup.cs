using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Models.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddTransient<IBoardThreadsRepository, EFRepository>();
            services.AddTransient<IBoardPostsRepository, EFRepository>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();
            app.UseStatusCodePages();
        }
    }
}
