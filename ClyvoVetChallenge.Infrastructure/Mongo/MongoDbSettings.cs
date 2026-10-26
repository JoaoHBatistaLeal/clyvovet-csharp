namespace ClyvoVetChallenge.Infrastructure.Mongo;

public class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "db_clyvovet_nosql";
}
