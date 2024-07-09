using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;

namespace PageTracker.Api;

public class Program
{
    public static Task Main(string[] args)
    {
        // Services
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
        });

        // Swagger
        builder.Services.AddSwaggerGen(options =>
        {
            // XML comments
            Assembly[] documentationAssemblies = [Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(Domain.AssemblyMarker))!];
            foreach (Assembly documentSource in documentationAssemblies)
            {
                var xmlFile = $"{documentSource.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            }

            options.ResolveConflictingActions(action => action.First());
            options.SwaggerDoc("v1.0", new OpenApiInfo { Title = "Page Tracker API", Version = "v1.0", Description = "Supporting the Page Tracker Client Application" });
        });

        // Use Api Versioning
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true; // Always fallback to version 1 if none specified
            o.DefaultApiVersion = new ApiVersion(1, 0); // Major, minor i.e. 1.0
            o.ReportApiVersions = true; // Adds discoverablility to new and deprecated versions
            o.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("api-version"), // e.g. /airports?api-version=2.0
                new HeaderApiVersionReader("x-version"), // Custom request header with key "x-version"
                new MediaTypeApiVersionReader("ver") // Version is in the Accept header e.g. application/json;ver=2.0
                );
        });


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1.0/swagger.json", "v1.0");
                options.DocumentTitle = "Page Tracker API";
            });
        }

        // Allow health monitoring check to bypass api key and region requirements
        app.MapGet("/", () => Results.Ok()).ExcludeFromDescription();

        app.UseAuthorization();

        app.MapControllers();

        return app.RunAsync();
    }
}
