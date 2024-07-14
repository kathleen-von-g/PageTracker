using Microsoft.Extensions.DependencyInjection;
using PageTracker.Application.ReadingSessions;

namespace PageTracker.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReadingSessions(this IServiceCollection services)
    {
        services.AddScoped<IReadingSessionService, ReadingSessionService>();
        return services;
    }
}
