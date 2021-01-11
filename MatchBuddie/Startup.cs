using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.EntityFrameworkCore;
using Application.Repositories;
using System.IO;
using DataLayer.Data;

namespace MatchBuddie
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

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInterestRepository, InterestRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IFriendRepository, FriendRepository>();
            services.AddScoped<IPostRepository, PostRepository>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            string workingDirectory = Environment.CurrentDirectory;
            string path = Directory.GetParent(Environment.CurrentDirectory).ToString();

            services.AddDbContext<MatchBuddieContext>(
                b => b.UseLazyLoadingProxies()
                        .UseSqlServer(Configuration.GetConnectionString("MatchBuddies1_0_1bContextConnection")
                        .Replace("[DataDirectory]", path)));

            services.AddControllersWithViews()
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    x.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStatusCodePagesWithReExecute("");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("");
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
                    pattern: "{controller=ShowcaseUsers}/{action=Index}/{search?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
