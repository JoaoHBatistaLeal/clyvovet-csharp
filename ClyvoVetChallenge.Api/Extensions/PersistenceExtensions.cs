using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Application.Services.Interfaces;
using ClyvoVetChallenge.Infrastructure.Persistence;
using ClyvoVetChallenge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVetChallenge.Api.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
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

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAnimalService, AnimalService>();
        services.AddScoped<IClinicaService, ClinicaService>();
        services.AddScoped<IConsultaService, ConsultaService>();
        services.AddScoped<IDiarioService, DiarioService>();
        services.AddScoped<IDonoService, DonoService>();
        services.AddScoped<IHistoricoService, HistoricoService>();
        services.AddScoped<IPrescricaoService, PrescricaoService>();
        services.AddScoped<ISintomaService, SintomaService>();
        services.AddScoped<IVeterinarioService, VeterinarioService>();

        return services;
    }

    public static IServiceCollection AddDBContext(this IServiceCollection services, IConfiguration configuration)
    {
        var useSqlite = configuration.GetValue<bool>("Database:UseSqlite");
        if (useSqlite)
        {
            var sqliteConnectionString = configuration.GetConnectionString("ClyvoVetSqlite")
                ?? "Data Source=clyvovet-dev.db";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(sqliteConnectionString));

            return services;
        }

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseNpgsql(connectionString);
            }
        });

        return services;
    }
}