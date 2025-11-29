using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CityDiscovery.Venues.Infrastructure.Security;

public static class JwtConfiguration
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                ClockSkew = TimeSpan.Zero
            };

            // Token'dan User bilgilerini çıkarmak için
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    // Token içinden userId'yi al ve HttpContext'e ekle
                    var userId = context.Principal?.FindFirst("sub")?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        context.HttpContext.Items["UserId"] = Guid.Parse(userId);
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            // Owner Policy - Sadece Owner rolündekiler
            options.AddPolicy("OwnerOnly", policy =>
                policy.RequireRole("Owner"));

            // Admin Policy - Sadece Admin rolündekiler
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));

            // Admin veya Owner
            options.AddPolicy("AdminOrOwner", policy =>
                policy.RequireRole("Admin", "Owner"));
        });

        return services;
    }
}