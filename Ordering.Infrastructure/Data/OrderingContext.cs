﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Infrastructure.Identity;
using System.Reflection.Emit;

namespace Ordering.Infrastructure.Data
{
    // Context class for command
    //public class OrderingContext : DbContext
    public class OrderingContext : IdentityDbContext<ApplicationUser>
    {
        public OrderingContext(DbContextOptions<OrderingContext> options) : base (options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
