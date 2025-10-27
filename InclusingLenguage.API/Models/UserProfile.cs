using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InclusingLenguage.API.Models
{
    public class UserProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("usuarioID")]
        public string UsuarioID { get; set; } = string.Empty;

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("correo")]
        public string Correo { get; set; } = string.Empty;

        [BsonElement("fechaRegistro")]
        public string FechaRegistro { get; set; } = string.Empty;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        // Propiedades adicionales para compatibilidad con la app
        [BsonIgnore]
        public string Email => Correo;

        [BsonIgnore]
        public string Name => Nombre;

        [BsonIgnore]
        public string FirstName { get; set; } = string.Empty;

        [BsonIgnore]
        public string LastName { get; set; } = string.Empty;

        [BsonIgnore]
        public int Level { get; set; } = 1;

        [BsonIgnore]
        public int Experience { get; set; } = 0;

        [BsonIgnore]
        public int Streak { get; set; } = 0;

        [BsonIgnore]
        public DateTime LastLogin { get; set; }

        [BsonIgnore]
        public DateTime CreatedAt { get; set; }

        [BsonIgnore]
        public List<string> CompletedLessons { get; set; } = new List<string>();

        [BsonIgnore]
        public Dictionary<string, double> LessonProgress { get; set; } = new Dictionary<string, double>();

        [BsonIgnore]
        public int DailyGoal { get; set; } = 5;

        [BsonIgnore]
        public int TodayProgress { get; set; } = 0;

        [BsonIgnore]
        public string ProfilePicture { get; set; } = string.Empty;

        [BsonIgnore]
        public bool IsGuest { get; set; } = false;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public UserProfile? UserProfile { get; set; }
    }
}
