using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using DataLayer.Data;
using DataLayer.Data.Models;

[assembly: HostingStartup(typeof(MatchBuddie.Areas.Identity.IdentityHostingStartup))]
namespace MatchBuddie.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string path = Directory.GetParent(Environment.CurrentDirectory).ToString();

            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MatchBuddieContext>(
                    b => b.UseLazyLoadingProxies()
                    .UseSqlServer(
                        context.Configuration.GetConnectionString("MatchBuddies1_0_1bContextConnection")
                                             .Replace("[DataDirectory]", path)));

                services.AddDefaultIdentity<User>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                }).AddEntityFrameworkStores<MatchBuddieContext>();
            });
        }
    }
}