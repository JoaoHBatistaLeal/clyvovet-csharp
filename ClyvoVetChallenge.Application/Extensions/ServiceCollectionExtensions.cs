using ClyvoVetChallenge.Application.Services.Implementations;
using ClyvoVetChallenge.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ClyvoVetChallenge.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
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
        services.AddScoped<IRecomendacaoIaService, RecomendacaoIaService>();

        return services;
    }
}
