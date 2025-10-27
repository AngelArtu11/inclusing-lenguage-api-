using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InclusingLenguage.API.Models
{
    public class Nivel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nivelID")]
        public int NivelID { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("maxIntentos")]
        public int MaxIntentos { get; set; } = 10;

        [BsonElement("recompensa")]
        public Recompensa Recompensa { get; set; } = new Recompensa();
    }

    public class Recompensa
    {
        [BsonElement("puntos")]
        public int Puntos { get; set; } = 0;
    }
}
