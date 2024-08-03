using Microsoft.Extensions.DependencyInjection;
using PageTracker.Application.Books;
using PageTracker.Application.ReadingSessions;

namespace PageTracker.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddReadingSessions();
        services.AddBooks();
        return services;
    }

    public static IServiceCollection AddReadingSessions(this IServiceCollection services)
    {
        services.AddScoped<IReadingSessionService, ReadingSessionService>();
        return services;
    }

    public static IServiceCollection AddBooks(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        return services;
    }
}
