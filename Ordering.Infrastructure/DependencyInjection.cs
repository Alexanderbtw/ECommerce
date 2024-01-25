using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Common.Interfaces;
using Ordering.Core.Repositories.Command.Base;
using Ordering.Core.Repositories.Query.Base;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Identity;
using Ordering.Infrastructure.Repository.Command.Base;
using Ordering.Infrastructure.Repository.Query.Base;
using Ordering.Infrastructure.Services;

namespace Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            string host = Environment.GetEnvironmentVariable("DB_HOST")!;
            string port = Environment.GetEnvironmentVariable("DB_PORT")!;
            string db_name = Environment.GetEnvironmentVariable("DB_DATABASE")!;
            string db_username = Environment.GetEnvironmentVariable("DB_USERNAME")!;
            string db_password = Environment.GetEnvironmentVariable("DB_PASSWORD")!;
            string db_setting = String.Format("Host={0}; Port={1}; User ID={2}; Password={3}; Database={4}; CommandTimeout=300;", host, port, db_username, db_password, db_name);

            services.AddDbContext<OrderingContext>(options => options.UseNpgsql(db_setting,
                b => b.MigrationsAssembly(typeof(OrderingContext).Assembly.FullName)
                ));

            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<OrderingContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.RequireUniqueEmail = true;
            });


            services.AddScoped<IIdentityService, IdentityService>();

            services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
            services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));

            return services;
        }
    }
}
