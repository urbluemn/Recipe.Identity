using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Recipe.Identity.Models;

namespace Recipe.Identity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AuthDbContext context)
        {
            // context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}