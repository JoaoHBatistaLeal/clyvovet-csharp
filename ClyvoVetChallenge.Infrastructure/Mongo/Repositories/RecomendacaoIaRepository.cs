using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Domain.Entities;
using ClyvoVetChallenge.Infrastructure.Mongo.Documents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClyvoVetChallenge.Infrastructure.Mongo.Repositories;

public class RecomendacaoIaRepository : IRecomendacaoIaRepository
{
    private readonly IMongoCollection<HistoricoRecomendacaoIaDocument>? _collection;
    private readonly ILogger<RecomendacaoIaRepository> _logger;
    private static readonly List<HistoricoRecomendacaoIa> _inMemoryStore = [];

    public RecomendacaoIaRepository(IOptions<MongoDbSettings> settings, ILogger<RecomendacaoIaRepository> logger)
    {
        _logger = logger;
        try
        {
            var config = settings.Value;
            var connectionString = config.ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = Environment.GetEnvironmentVariable("MongoDbSettings__ConnectionString")
                    ?? Environment.GetEnvironmentVariable("MONGODB_URI")
                    ?? string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(config.DatabaseName);
                _collection = database.GetCollection<HistoricoRecomendacaoIaDocument>("recomendacoes_ia");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Nao foi possivel inicializar conexao com MongoDB para RecomendacaoIaRepository.");
        }
    }

    public async Task<IReadOnlyCollection<HistoricoRecomendacaoIa>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_collection != null)
        {
            try
            {
                var docs = await _collection.Find(_ => true).ToListAsync(cancellationToken);
                return docs.Select(ToEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao buscar todas as recomendacoes no MongoDB. Utilizando memoria.");
            }
        }

        return _inMemoryStore.ToList();
    }

    public async Task<HistoricoRecomendacaoIa?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (_collection != null)
        {
            try
            {
                var filter = Builders<HistoricoRecomendacaoIaDocument>.Filter.Eq(x => x.Id, id);
                var doc = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
                return doc != null ? ToEntity(doc) : null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao buscar recomendacao no MongoDB por id {Id}.", id);
            }
        }

        return _inMemoryStore.FirstOrDefault(x => x.Id == id);
    }

    public async Task<IReadOnlyCollection<HistoricoRecomendacaoIa>> GetByAnimalIdAsync(Guid animalId, CancellationToken cancellationToken = default)
    {
        var animalIdStr = animalId.ToString();
        if (_collection != null)
        {
            try
            {
                var filter = Builders<HistoricoRecomendacaoIaDocument>.Filter.Eq(x => x.AnimalId, animalIdStr);
                var docs = await _collection.Find(filter).ToListAsync(cancellationToken);
                return docs.Select(ToEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao buscar recomendacoes do animal {AnimalId} no MongoDB.", animalId);
            }
        }

        return _inMemoryStore.Where(x => x.AnimalId == animalId).ToList();
    }

    public async Task CreateAsync(HistoricoRecomendacaoIa entity, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            entity.Id = Guid.NewGuid().ToString("N");
        }

        if (_collection != null)
        {
            try
            {
                var doc = ToDocument(entity);
                await _collection.InsertOneAsync(doc, cancellationToken: cancellationToken);
                entity.Id = doc.Id ?? entity.Id;
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao inserir recomendacao no MongoDB. Gravando em memoria.");
            }
        }

        _inMemoryStore.Add(entity);
    }

    private static HistoricoRecomendacaoIa ToEntity(HistoricoRecomendacaoIaDocument doc)
    {
        _ = Guid.TryParse(doc.AnimalId, out var animalId);
        return new HistoricoRecomendacaoIa
        {
            Id = doc.Id ?? string.Empty,
            AnimalId = animalId,
            NomeAnimal = doc.NomeAnimal,
            Especie = doc.Especie,
            IdadeAnos = doc.IdadeAnos,
            SintomasRelatados = doc.SintomasRelatados,
            DiagnosticoSugerido = doc.DiagnosticoSugerido,
            NivelUrgencia = doc.NivelUrgencia,
            RecomendacoesCuidados = doc.RecomendacoesCuidados,
            DataAvaliacao = doc.DataAvaliacao,
            ModeloIaUtilizado = doc.ModeloIaUtilizado
        };
    }

    private static HistoricoRecomendacaoIaDocument ToDocument(HistoricoRecomendacaoIa entity)
    {
        return new HistoricoRecomendacaoIaDocument
        {
            Id = !string.IsNullOrEmpty(entity.Id) && entity.Id.Length == 24 ? entity.Id : null,
            AnimalId = entity.AnimalId.ToString(),
            NomeAnimal = entity.NomeAnimal,
            Especie = entity.Especie,
            IdadeAnos = entity.IdadeAnos,
            SintomasRelatados = entity.SintomasRelatados,
            DiagnosticoSugerido = entity.DiagnosticoSugerido,
            NivelUrgencia = entity.NivelUrgencia,
            RecomendacoesCuidados = entity.RecomendacoesCuidados,
            DataAvaliacao = entity.DataAvaliacao,
            ModeloIaUtilizado = entity.ModeloIaUtilizado
        };
    }
}
