using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace ClyvoVetChallenge.Api.Exceptions;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        logger.LogError(exception, "Excecao nao tratada capturada: {Message} | TraceId: {TraceId}", exception.Message, traceId);

        var (statusCode, title, detail) = MapException(environment, exception);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = "about:blank",
            Title = title,
            Status = statusCode,
            Detail = environment.IsDevelopment() ? detail : "Ocorreu um erro inesperado.",
            Instance = httpContext.Request.Path
        };

        if (environment.IsDevelopment())
        {
            problem.Extensions["traceId"] = traceId;
        }

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Title, string? Detail) MapException(IHostEnvironment environment, Exception exception)
    {
        return exception switch
        {
            ArgumentNullException e => (StatusCodes.Status400BadRequest, "Requisicao invalida", e.Message),
            ArgumentException e => (StatusCodes.Status400BadRequest, "Requisicao invalida", e.Message),
            InvalidOperationException e => (StatusCodes.Status400BadRequest, "Nao foi possivel concluir a operacao", e.Message),
            KeyNotFoundException e => (StatusCodes.Status404NotFound, "Recurso nao encontrado", e.Message),
            UnauthorizedAccessException e => (StatusCodes.Status401Unauthorized, "Nao autorizado", e.Message),
            NpgsqlException e => (StatusCodes.Status500InternalServerError, "Erro de comunicacao com o Banco de Dados", e.Message),
            _ => MapUnhandled(environment, exception)
        };
    }

    private static (int StatusCode, string Title, string? Detail) MapUnhandled(IHostEnvironment environment, Exception exception)
    {
        return environment.IsDevelopment()
            ? (StatusCodes.Status500InternalServerError, "Erro interno do servidor", exception.ToString())
            : (StatusCodes.Status500InternalServerError, "Erro interno do servidor", "Ocorreu um erro inesperado.");
    }
}