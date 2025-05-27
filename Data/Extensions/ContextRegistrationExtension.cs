using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extensions;

public static class ContextRegistrationExtension
{
    public static IServiceCollection AddContexts(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connectionString));
        return services;
    }
}