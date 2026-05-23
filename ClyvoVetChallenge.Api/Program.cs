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
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<ClyvoVetContext>();

    int tentativas = 0;
    bool migrado = false;

    while (tentativas < 12 && !migrado)
    {
        try
        {
            context.Database.Migrate();
            migrado = true;
        }
        catch (Exception)
        {
            tentativas++;
            Thread.Sleep(15000);
        }
    }
}

app.Run();