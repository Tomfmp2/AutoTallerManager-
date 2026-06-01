using Application.Abstractions;
using Infrastructure.Data;
using Infrastructure.Data.Context;
using Infrastructure.Identity;
using Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
       this IServiceCollection services,
       IConfiguration configuration)
    {
        services.AddDbContext<AutoTallerDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(AutoTallerDbContext).Assembly.FullName)
            ));
            
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<AutoTallerDbContext>());
            
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        
        return services;
    }
}
