using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ordering.Application.Commands.Role;
using Ordering.Application.Commands.User.Create;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Security;
using Ordering.Application.Handlers.CommandHandler.User.Create;
using Ordering.Application.Queries.Role;
using Ordering.Infrastructure;
using System.Text;
//using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseUrls("http://0.0.0.0:7142");
// Add services to the container.
builder.Services.AddControllers();

// For authentication
var _key = builder.Configuration["Jwt:Key"];
var _issuer = builder.Configuration["Jwt:Issuer"];
var _audience = builder.Configuration["Jwt:Audience"];
var _expirtyMinutes = builder.Configuration["Jwt:ExpiryMinutes"];

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = _audience,
        ValidIssuer = _issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
        ValidateIssuerSigningKey = true,
    };
});

// Dependency injection with key
builder.Services.AddSingleton<ITokenGenerator>(new TokenGenerator(_key, _issuer, _audience, _expirtyMinutes));
//builder.Services.AddSingleton<ITokenGenerator>(new TokenGenerator(_key));

// Include Infrastructur Dependency
builder.Services.AddInfrastructure(builder.Configuration);

// Register dependencies
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateUserCommandHandler>());

//Enable CORS//Cross site resource sharing
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        //.AllowCredentials()
        );
});


//builder.Services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
//builder.Services.AddTransient<ICustomerQueryRepository, CustomerQueryRepository>();
//builder.Services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));
//builder.Services.AddTransient<ICustomerCommandRepository, CustomerCommandRepository>();
//builder.Services.AddScoped<IIdentityService, IdentityService>();

//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<OrderingContext>()
//    .AddDefaultTokenProviders();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    //c.AddServer(new OpenApiServer { Url = "https://localhost:7142/swagger" });

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auth API",
        Version = "v1",
        Description = "API for authorization and authentification",
    });
    // To enable authorization using swagger (Jwt)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer {token}\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API V1");
});

app.UseHttpsRedirection();

//Must be betwwen app.UseRouting() and app.UseEndPoints()
app.UseCors("CorsPolicy");

// Added for authentication
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    var ans = await mediator.Send(new GetRoleQuery());

    if (ans == null || ans.Count == 0)
    {
        await mediator.Send(new RoleCreateCommand { RoleName = "Admin" });
        await mediator.Send(new RoleCreateCommand { RoleName = "User" });

        await mediator.Send(new CreateUserCommand
        {
            Email = "admin@gmail.com",
            UserName = "Admin",
            Password = "Admin@1",
            ConfirmationPassword = "Admin@1",
            Roles = ["Admin"]
        });
    }
}

app.Run();
