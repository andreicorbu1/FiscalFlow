using System.Text;
using FiscalFlow.Application.Core.Abstractions.Authentication;
using FiscalFlow.Application.Core.Abstractions.Emails;
using FiscalFlow.Application.Services;
using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Repositories;
using FiscalFlow.Infrastructure.Authentication;
using FiscalFlow.Infrastructure.Persistence;
using FiscalFlow.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        });
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!)),
                    ValidIssuer = config["JWT:Issuer"],
                    ValidAudience = config["JWT:Audience"],
                    ValidateIssuer = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                    ValidateAudience = false
                };
            })
            .AddGoogle(opts =>
            {
                opts.ClientId = config["Google:ClientId"]!;
                opts.ClientSecret = config["Google:SecretId"]!;
                opts.SignInScheme = IdentityConstants.ExternalScheme;
            });
        services.AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddUserManager<UserManager<AppUser>>()
            .AddDefaultTokenProviders();

        services.AddCors();
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = actionsContext =>
            {
                var errors = actionsContext.ModelState.Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value?.Errors!)
                    .Select(x => x.ErrorMessage).ToArray();

                var toReturn = new
                {
                    Errors = errors
                };

                return new BadRequestObjectResult(toReturn);
            };
        });
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        return services;
    }
}