using ClyvoVetChallenge.Application.Configurations;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Interfaces;
using ClyvoVetChallenge.Infrastructure.Mongo;
using ClyvoVetChallenge.Infrastructure.Mongo.Repositories;
using ClyvoVetChallenge.Infrastructure.Persistence;
using ClyvoVetChallenge.Infrastructure.Repositories;
using ClyvoVetChallenge.Infrastructure.Services;
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

        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));
        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        services.AddScoped<IRecomendacaoIaRepository, RecomendacaoIaRepository>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<ITokenService, TokenService>();

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
