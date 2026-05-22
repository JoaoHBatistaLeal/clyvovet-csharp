using ClyvoVetChallenge.Api.Extensions;
using ClyvoVetChallenge.Api.Exceptions;
using DotNetEnv;
using System.Reflection;
using ClyvoVetChallenge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDBContext(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ClyvoVetContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrations no banco de dados.");
    }
}

app.Run();