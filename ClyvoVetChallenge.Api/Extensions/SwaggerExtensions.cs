using System.Reflection;
using Microsoft.OpenApi.Models;

namespace ClyvoVetChallenge.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerCustom(this IServiceCollection services)
    {
        return services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "lyvoVetChallenge API",
                Version = "v1",
                Description = "API de gerenciamento para o desafio técnico ClyvoVet."
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        });
    }
}