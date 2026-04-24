using Book.Application.Abstractions.Persistence;
using Book.Application.Services.Assuntos;
using Book.Application.Services.Autores;
using Book.Application.Services.Livros;
using Book.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Book.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IBookDbConnectionFactory>(_ =>
            new SqlServerBookDbConnectionFactory(
                configuration.GetConnectionString("DefaultConnection")
                ?? "Server=localhost,1433;Database=BookDb;User Id=sa;Password=Book@123456;TrustServerCertificate=True;"));
        services.AddScoped<IAssuntoRepository, AssuntoRepository>();
        services.AddScoped<IAutorRepository, AutorRepository>();
        services.AddScoped<ILivroRepository, LivroRepository>();
        services.AddScoped<IAssuntoAppService, AssuntoAppService>();
        services.AddScoped<IAutorAppService, AutorAppService>();
        services.AddScoped<ILivroAppService, LivroAppService>();

        return services;
    }
}
