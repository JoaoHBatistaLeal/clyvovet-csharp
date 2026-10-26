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
                Title = "Clyvo Vet API",
                Version = "1.0.0-sprint4",
                Description = "API RESTful Clyvo Vet para gestao de clinicas veterinarias, consultas, animais, IA clinica e historico medico.",
                Contact = new OpenApiContact
                {
                    Name = "Equipe Clyvo Vet",
                    Email = "contato@clyvovet.com"
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT no formato: Bearer {seu_token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
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
