using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Recipe.Identity.Data
{
   public static class DependencyInjection
    {
        /// <summary>
        /// Configuring Dependency Injection Extension for DB
        /// </summary>
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            //SQLite db
            // var connectionString = configuration["DbConnection"];

            //SQLServer db
            var connectionString = configuration["SqlServerDbConnect"];
            services.AddDbContext<AuthDbContext>(opts =>
                opts.UseSqlServer(connectionString));
            services.AddScoped<AuthDbContext>(/*provider => provider.GetService<RecipeDbContext>()*/);
            return services;
        }
    }
}