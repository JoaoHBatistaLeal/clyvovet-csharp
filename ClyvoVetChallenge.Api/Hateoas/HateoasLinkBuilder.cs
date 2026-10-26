using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.DTOs.Common;

namespace ClyvoVetChallenge.Api.Hateoas;

public static class HateoasLinkBuilder
{
    public static void AddAnimalLinks(AnimalResponse response, HttpContext httpContext)
    {
        var scheme = httpContext.Request.Scheme;
        var host = httpContext.Request.Host.Value;
        var baseUrl = $"{scheme}://{host}/api/Animal/{response.Id}";

        response.Links =
        [
            new LinkDto(baseUrl, "self", "GET"),
            new LinkDto(baseUrl, "update", "PUT"),
            new LinkDto(baseUrl, "delete", "DELETE"),
            new LinkDto($"{scheme}://{host}/api/Animal", "all-animals", "GET")
        ];
    }

    public static void AddPagedAnimalLinks(PagedResponse<AnimalResponse> pagedResponse, AnimalQueryParameters parameters, HttpContext httpContext)
    {
        var scheme = httpContext.Request.Scheme;
        var host = httpContext.Request.Host.Value;
        var baseUrl = $"{scheme}://{host}/api/Animal/paged";

        var links = new List<LinkDto>
        {
            new LinkDto(BuildAnimalUrl(baseUrl, parameters.PageNumber, parameters.PageSize, parameters), "self", "GET"),
            new LinkDto(BuildAnimalUrl(baseUrl, 1, parameters.PageSize, parameters), "first-page", "GET"),
            new LinkDto(BuildAnimalUrl(baseUrl, Math.Max(1, pagedResponse.TotalPages), parameters.PageSize, parameters), "last-page", "GET")
        };

        if (pagedResponse.HasPreviousPage)
        {
            links.Add(new LinkDto(BuildAnimalUrl(baseUrl, parameters.PageNumber - 1, parameters.PageSize, parameters), "previous-page", "GET"));
        }

        if (pagedResponse.HasNextPage)
        {
            links.Add(new LinkDto(BuildAnimalUrl(baseUrl, parameters.PageNumber + 1, parameters.PageSize, parameters), "next-page", "GET"));
        }

        pagedResponse.Links = links;

        foreach (var item in pagedResponse.Items)
        {
            AddAnimalLinks(item, httpContext);
        }
    }

    public static void AddConsultaLinks(ConsultaResponse response, HttpContext httpContext)
    {
        var scheme = httpContext.Request.Scheme;
        var host = httpContext.Request.Host.Value;
        var baseUrl = $"{scheme}://{host}/api/Consulta/{response.Id}";

        response.Links =
        [
            new LinkDto(baseUrl, "self", "GET"),
            new LinkDto(baseUrl, "update", "PUT"),
            new LinkDto(baseUrl, "delete", "DELETE"),
            new LinkDto($"{scheme}://{host}/api/Consulta", "all-consultas", "GET")
        ];
    }

    public static void AddPagedConsultaLinks(PagedResponse<ConsultaResponse> pagedResponse, ConsultaQueryParameters parameters, HttpContext httpContext)
    {
        var scheme = httpContext.Request.Scheme;
        var host = httpContext.Request.Host.Value;
        var baseUrl = $"{scheme}://{host}/api/Consulta/paged";

        var links = new List<LinkDto>
        {
            new LinkDto(BuildConsultaUrl(baseUrl, parameters.PageNumber, parameters.PageSize, parameters), "self", "GET"),
            new LinkDto(BuildConsultaUrl(baseUrl, 1, parameters.PageSize, parameters), "first-page", "GET"),
            new LinkDto(BuildConsultaUrl(baseUrl, Math.Max(1, pagedResponse.TotalPages), parameters.PageSize, parameters), "last-page", "GET")
        };

        if (pagedResponse.HasPreviousPage)
        {
            links.Add(new LinkDto(BuildConsultaUrl(baseUrl, parameters.PageNumber - 1, parameters.PageSize, parameters), "previous-page", "GET"));
        }

        if (pagedResponse.HasNextPage)
        {
            links.Add(new LinkDto(BuildConsultaUrl(baseUrl, parameters.PageNumber + 1, parameters.PageSize, parameters), "next-page", "GET"));
        }

        pagedResponse.Links = links;

        foreach (var item in pagedResponse.Items)
        {
            AddConsultaLinks(item, httpContext);
        }
    }

    private static string BuildAnimalUrl(string baseUrl, int pageNumber, int pageSize, AnimalQueryParameters p)
    {
        return $"{baseUrl}?pageNumber={pageNumber}&pageSize={pageSize}" +
               (string.IsNullOrWhiteSpace(p.Nome) ? "" : $"&nome={Uri.EscapeDataString(p.Nome)}") +
               (string.IsNullOrWhiteSpace(p.Especie) ? "" : $"&especie={Uri.EscapeDataString(p.Especie)}") +
               (string.IsNullOrWhiteSpace(p.SortBy) ? "" : $"&sortBy={Uri.EscapeDataString(p.SortBy)}&sortDescending={p.SortDescending}");
    }

    private static string BuildConsultaUrl(string baseUrl, int pageNumber, int pageSize, ConsultaQueryParameters p)
    {
        return $"{baseUrl}?pageNumber={pageNumber}&pageSize={pageSize}" +
               (p.AnimalId.HasValue ? $"&animalId={p.AnimalId.Value}" : "") +
               (string.IsNullOrWhiteSpace(p.Status) ? "" : $"&status={Uri.EscapeDataString(p.Status)}") +
               (string.IsNullOrWhiteSpace(p.SortBy) ? "" : $"&sortBy={Uri.EscapeDataString(p.SortBy)}&sortDescending={p.SortDescending}");
    }
}
