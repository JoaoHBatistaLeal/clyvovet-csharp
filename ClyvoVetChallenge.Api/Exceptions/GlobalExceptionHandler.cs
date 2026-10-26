using System.Diagnostics;
using ClyvoVetChallenge.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClyvoVetChallenge.Api.Exceptions;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        logger.LogError(exception, "Excecao capturada no pipeline HTTP: {Message} | TraceId: {TraceId}", exception.Message, traceId);

        var (statusCode, title, detail) = MapException(environment, exception);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7807",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = traceId;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(IHostEnvironment environment, Exception exception)
    {
        return exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, "Recurso nao encontrado", e.Message),
            KeyNotFoundException e => (StatusCodes.Status404NotFound, "Recurso nao encontrado", e.Message),
            BusinessRuleException e => (StatusCodes.Status400BadRequest, "Regra de negocio violada", e.Message),
            DomainException e => (StatusCodes.Status400BadRequest, "Erro de validacao de dominio", e.Message),
            ArgumentNullException e => (StatusCodes.Status400BadRequest, "Requisicao invalida", e.Message),
            ArgumentException e => (StatusCodes.Status400BadRequest, "Requisicao invalida", e.Message),
            InvalidOperationException e => (StatusCodes.Status400BadRequest, "Operacao invalida", e.Message),
            UnauthorizedAccessException e => (StatusCodes.Status401Unauthorized, "Acesso nao autorizado", e.Message),
            _ => MapUnhandled(environment, exception)
        };
    }

    private static (int StatusCode, string Title, string Detail) MapUnhandled(IHostEnvironment environment, Exception exception)
    {
        return environment.IsDevelopment()
            ? (StatusCodes.Status500InternalServerError, "Erro interno do servidor", exception.ToString())
            : (StatusCodes.Status500InternalServerError, "Erro interno do servidor", "Ocorreu um erro inesperado no processamento da requisicao.");
    }
}
