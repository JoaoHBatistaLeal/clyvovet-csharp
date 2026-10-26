using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClyvoVetChallenge.Infrastructure.Mongo.Documents;

public class HistoricoRecomendacaoIaDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("animalId")]
    public string AnimalId { get; set; } = string.Empty;

    [BsonElement("nomeAnimal")]
    public string NomeAnimal { get; set; } = string.Empty;

    [BsonElement("especie")]
    public string Especie { get; set; } = string.Empty;

    [BsonElement("idadeAnos")]
    public int IdadeAnos { get; set; }

    [BsonElement("sintomasRelatados")]
    public List<string> SintomasRelatados { get; set; } = [];

    [BsonElement("diagnosticoSugerido")]
    public string DiagnosticoSugerido { get; set; } = string.Empty;

    [BsonElement("nivelUrgencia")]
    public string NivelUrgencia { get; set; } = string.Empty;

    [BsonElement("recomendacoesCuidados")]
    public List<string> RecomendacoesCuidados { get; set; } = [];

    [BsonElement("dataAvaliacao")]
    public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;

    [BsonElement("modeloIaUtilizado")]
    public string ModeloIaUtilizado { get; set; } = "ClyvoVet-ClinicalAI-v2";
}
