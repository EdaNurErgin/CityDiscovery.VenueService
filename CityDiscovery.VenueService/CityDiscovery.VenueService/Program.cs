using CityDiscovery.Venues.Application.DependencyInjection;
using CityDiscovery.Venues.Application.Interfaces.Services;
using CityDiscovery.Venues.Infrastructure.DependencyInjection;
using CityDiscovery.Venues.Infrastructure.Middleware;
using CityDiscovery.Venues.Infrastructure.Security;
using CityDiscovery.Venues.Infrastructure.Services;
using CityDiscovery.Venues.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.Configure<LocalFileStorageOptions>(
    builder.Configuration.GetSection("LocalFileStorage"));

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();


builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddScoped<IAuthorizationHandler, VenueOwnerAuthorizationHandler>();

builder.Services.AddVenueApplication();
builder.Services.AddVenueInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Venue Service API",
        Version = "v1",
        Description = "CityDiscovery Venue Service API - Mekan yönetimi, menü, etkinlik ve fotoğraf işlemleri için RESTful API",
        Contact = new OpenApiContact
        {
            Name = "CityDiscovery Team"
        }
    });

    // XML dokümantasyon dosyasını ekle
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Request/Response örnekleri ve Swagger annotations için
    c.EnableAnnotations();
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. �rnek: \"Bearer {token}\""
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
            Array.Empty<string>()
        }
    });
});

// Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// ==========================================================================
// Database Migration at Startup
// ==========================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Starting database migration...");
        var dbContext = services.GetRequiredService<VenueDbContext>();
        dbContext.Database.Migrate();
        logger.LogInformation("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw; // Re-throw to prevent app from starting with broken DB
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

var uploadRootPath = builder.Configuration["LocalFileStorage:RootPath"];
if (!string.IsNullOrWhiteSpace(uploadRootPath) && Path.IsPathRooted(uploadRootPath) && Directory.Exists(uploadRootPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(uploadRootPath),
        RequestPath = "/uploads"
    });
}

app.UseAuthentication();
app.UseAuthorization();

// Health Check Endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
