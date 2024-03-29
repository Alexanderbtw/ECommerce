﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Identity;

namespace Ordering.Infrastructure.Data
{
    // Context class for command
    //public class OrderingContext : DbContext
    public class OrderingContext : IdentityDbContext<ApplicationUser>
    {
        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
