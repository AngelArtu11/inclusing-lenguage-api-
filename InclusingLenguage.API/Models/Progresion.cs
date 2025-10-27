using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InclusingLenguage.API.Models
{
    public class Progresion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("usuarioID")]
        public string UsuarioID { get; set; } = string.Empty;

        [BsonElement("nivelActual")]
        public int NivelActual { get; set; } = 0;

        [BsonElement("nivelesCompletados")]
        public List<int> NivelesCompletados { get; set; } = new List<int>();

        [BsonElement("intentos")]
        public List<Intento> Intentos { get; set; } = new List<Intento>();

        [BsonElement("estadisticas")]
        public Estadisticas Estadisticas { get; set; } = new Estadisticas();
    }

    public class Intento
    {
        [BsonElement("nivel")]
        public int Nivel { get; set; }

        [BsonElement("resultado")]
        public string Resultado { get; set; } = string.Empty; // "exito" o "fallo"

        [BsonElement("fecha")]
        public string Fecha { get; set; } = string.Empty;
    }

    public class Estadisticas
    {
        [BsonElement("tiempoJugadoMin")]
        public int TiempoJugadoMin { get; set; } = 0;

        [BsonElement("totalIntentos")]
        public int TotalIntentos { get; set; } = 0;

        [BsonElement("totalExitos")]
        public int TotalExitos { get; set; } = 0;
    }
}
