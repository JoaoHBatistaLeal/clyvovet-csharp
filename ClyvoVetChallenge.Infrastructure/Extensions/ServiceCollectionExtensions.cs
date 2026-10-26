using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Infrastructure.Persistence;
using ClyvoVetChallenge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClyvoVetChallenge.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var useSqlite = configuration.GetValue<bool>("Database:UseSqlite");
        if (useSqlite)
        {
            var sqliteConnectionString = configuration.GetConnectionString("ClyvoVetSqlite")
                ?? "Data Source=clyvovet-dev.db";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(sqliteConnectionString));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseNpgsql(connectionString);
                }
            });
        }

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<IClinicaRepository, ClinicaRepository>();
        services.AddScoped<IConsultaRepository, ConsultaRepository>();
        services.AddScoped<IDiarioRepository, DiarioRepository>();
        services.AddScoped<IDiarioSintomaRepository, DiarioSintomaRepository>();
        services.AddScoped<IDonoRepository, DonoRepository>();
        services.AddScoped<IHistoricoRepository, HistoricoRepository>();
        services.AddScoped<IPrescricaoRepository, PrescricaoRepository>();
        services.AddScoped<ISintomaRepository, SintomaRepository>();
        services.AddScoped<IVeterinarioRepository, VeterinarioRepository>();

        return services;
    }
}
