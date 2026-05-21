using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client; 

namespace ClyvoVetChallenge.Api.Exceptions;

public class GlobalExceptionHandler(IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
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
        
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
    
    private static (int StatusCode, string title, string? Detail) MapException(IHostEnvironment environment, Exception exception)
    {
        return exception switch
        {
            ArgumentNullException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
            ArgumentException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
            InvalidOperationException e => (StatusCodes.Status400BadRequest, "Não foi possível concluir a operação", e.Message),
            KeyNotFoundException e => (StatusCodes.Status404NotFound, "Recurso não encontrado", e.Message),
            UnauthorizedAccessException e => (StatusCodes.Status401Unauthorized, "Não autorizado", e.Message),
            OracleException e => (StatusCodes.Status500InternalServerError, "Erro de comunicação com o Banco de Dados", e.Message),
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