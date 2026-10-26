using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ClyvoVetChallenge.Infrastructure.Mongo.Repositories;

public class MongoRepository<T> : IMongoRepository<T> where T : class
{
    private readonly IMongoCollection<T>? _collection;
    private readonly ILogger<MongoRepository<T>> _logger;
    private static readonly List<T> _inMemoryStore = [];

    public MongoRepository(IOptions<MongoDbSettings> settings, ILogger<MongoRepository<T>> logger)
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
                var collectionName = typeof(T).Name.Replace("Document", string.Empty).ToLowerInvariant();
                _collection = database.GetCollection<T>(collectionName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Nao foi possivel inicializar conexao com MongoDB. Operando com armazenamento em memoria temporario.");
        }
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_collection != null)
        {
            try
            {
                return await _collection.Find(_ => true).ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao consultar MongoDB. Recorrendo ao armazenamento em memoria.");
            }
        }

        return _inMemoryStore.ToList();
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (_collection != null)
        {
            try
            {
                var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
                return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao buscar documento no MongoDB por id {Id}.", id);
            }
        }

        return null;
    }

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (_collection != null)
        {
            try
            {
                await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao persistir no MongoDB. Salvando em memoria.");
            }
        }

        _inMemoryStore.Add(entity);
    }
}
