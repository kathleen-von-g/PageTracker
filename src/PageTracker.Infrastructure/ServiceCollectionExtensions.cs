using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PageTracker.Infrastructure.Persistence;

namespace PageTracker.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPageTrackerContext(this IServiceCollection services)
    {
        services.AddDbContext<PageTrackerDbContext>(options => { options.UseSqlServer("Name=ConnectionStrings:PageTracker");});
        services.AddScoped<IPageTrackerDbContext, PageTrackerDbContext>();

        return services;
    }
}
