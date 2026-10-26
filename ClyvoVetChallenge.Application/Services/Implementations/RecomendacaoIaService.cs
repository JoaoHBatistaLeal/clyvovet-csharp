using ClyvoVetChallenge.Application.DTOs;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Application.Services.Interfaces;
using ClyvoVetChallenge.Domain.Entities;
using ClyvoVetChallenge.Domain.Exceptions;

namespace ClyvoVetChallenge.Application.Services.Implementations;

public class RecomendacaoIaService(IRecomendacaoIaRepository recomendacaoIaRepository) : IRecomendacaoIaService
{
    public async Task<RecomendacaoIaResponse> GerarRecomendacaoAsync(RecomendacaoIaRequest request, CancellationToken cancellationToken = default)
    {
        if (request.AnimalId == Guid.Empty)
        {
            throw new BusinessRuleException("O identificador do animal e obrigatorio para geracao da recomendacao clinica.");
        }

        if (request.Sintomas == null || request.Sintomas.Count == 0)
        {
            throw new BusinessRuleException("Ao menos um sintoma clinico deve ser informado para analise.");
        }

        var sintomasNormalizados = request.Sintomas
            .Select(s => s.Trim().ToLowerInvariant())
            .ToList();

        var nivelUrgencia = DeterminarUrgencia(sintomasNormalizados);
        var diagnosticoSugerido = SugerirDiagnostico(request.Especie, sintomasNormalizados);
        var recomendacoes = GerarRecomendacoes(nivelUrgencia, sintomasNormalizados);

        var entity = new HistoricoRecomendacaoIa
        {
            AnimalId = request.AnimalId,
            NomeAnimal = request.NomeAnimal,
            Especie = request.Especie,
            IdadeAnos = request.IdadeAnos,
            SintomasRelatados = request.Sintomas,
            DiagnosticoSugerido = diagnosticoSugerido,
            NivelUrgencia = nivelUrgencia,
            RecomendacoesCuidados = recomendacoes,
            DataAvaliacao = DateTime.UtcNow,
            ModeloIaUtilizado = "ClyvoVet-ClinicalDecision-v2"
        };

        await recomendacaoIaRepository.CreateAsync(entity, cancellationToken);

        return ToResponse(entity);
    }

    public async Task<IReadOnlyCollection<RecomendacaoIaResponse>> ObterHistoricoPorAnimalAsync(Guid animalId, CancellationToken cancellationToken = default)
    {
        var historicos = await recomendacaoIaRepository.GetByAnimalIdAsync(animalId, cancellationToken);
        return historicos.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyCollection<RecomendacaoIaResponse>> ObterTodosHistoricosAsync(CancellationToken cancellationToken = default)
    {
        var historicos = await recomendacaoIaRepository.GetAllAsync(cancellationToken);
        return historicos.Select(ToResponse).ToList();
    }

    private static string DeterminarUrgencia(List<string> sintomas)
    {
        var criticos = new[] { "convulsao", "dificuldade respiratoria", "sangramento", "parada", "cianose", "hemorragia" };
        var altos = new[] { "febre", "vomito", "dor intensa", "prostracao", "inconsciencia" };
        var medios = new[] { "diarreia", "perda de apetite", "letargia", "tosse", "coxeira" };

        if (sintomas.Any(s => criticos.Any(c => s.Contains(c))))
        {
            return "Emergencia";
        }

        if (sintomas.Any(s => altos.Any(a => s.Contains(a))))
        {
            return "Alta";
        }

        if (sintomas.Any(s => medios.Any(m => s.Contains(m))))
        {
            return "Media";
        }

        return "Baixa";
    }

    private static string SugerirDiagnostico(string especie, List<string> sintomas)
    {
        var especieNorm = especie.ToLowerInvariant();
        if (sintomas.Any(s => s.Contains("dificuldade respiratoria") || s.Contains("tosse")))
        {
            return especieNorm.Contains("fel") ? "Suspeita de Complexo Respiratorio Felino / Asma Felina" : "Suspeita de Traqueobronquite Infecciosa Canina";
        }

        if (sintomas.Any(s => s.Contains("vomito") || s.Contains("diarreia")))
        {
            return "Gastroenterite com necessidade de hidratacao e suporte enterico";
        }

        if (sintomas.Any(s => s.Contains("coceira") || s.Contains("queda de pelos") || s.Contains("alergia")))
        {
            return "Dermatite alergica ou atopia cutanea sazonal";
        }

        return "Quadro clinico inespecifico; avaliacao presencial recomendada para exames laboratoriais complementares";
    }

    private static List<string> GerarRecomendacoes(string urgencia, List<string> sintomas)
    {
        var recomendacoes = new List<string>();

        switch (urgencia)
        {
            case "Emergencia":
                recomendacoes.Add("Encaminhamento imediato para unidade de pronto atendimento veterinario.");
                recomendacoes.Add("Manter o paciente em repouso e evitar movimentacao brusca.");
                break;
            case "Alta":
                recomendacoes.Add("Agendar consulta presencial em no maximo 24 horas.");
                recomendacoes.Add("Monitorar temperatura retal e frequencia hidrica.");
                break;
            case "Media":
                recomendacoes.Add("Manter alimentacao leve e hidratacao constante com agua fresca.");
                recomendacoes.Add("Registrar evolucao dos sintomas nas proximas 48 horas.");
                break;
            default:
                recomendacoes.Add("Monitorar comportamento rotineiro do pet.");
                recomendacoes.Add("Higienizacao e cuidados preventivos padrao de acordo com o protocolo da clinica.");
                break;
        }

        return recomendacoes;
    }

    private static RecomendacaoIaResponse ToResponse(HistoricoRecomendacaoIa entity)
    {
        return new RecomendacaoIaResponse
        {
            Id = entity.Id,
            AnimalId = entity.AnimalId,
            NomeAnimal = entity.NomeAnimal,
            Especie = entity.Especie,
            DiagnosticoSugerido = entity.DiagnosticoSugerido,
            NivelUrgencia = entity.NivelUrgencia,
            RecomendacoesCuidados = entity.RecomendacoesCuidados,
            DataAvaliacao = entity.DataAvaliacao,
            ModeloIaUtilizado = entity.ModeloIaUtilizado
        };
    }
}
