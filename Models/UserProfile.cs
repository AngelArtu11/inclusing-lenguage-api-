using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InclusingLenguage.API.Models
{
    public class UserProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("level")]
        public int Level { get; set; } = 1;

        [BsonElement("experience")]
        public int Experience { get; set; } = 0;

        [BsonElement("streak")]
        public int Streak { get; set; } = 0;

        [BsonElement("lastLogin")]
        public DateTime LastLogin { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("completedLessons")]
        public List<string> CompletedLessons { get; set; } = new List<string>();

        [BsonElement("lessonProgress")]
        public Dictionary<string, double> LessonProgress { get; set; } = new Dictionary<string, double>();

        [BsonElement("dailyGoal")]
        public int DailyGoal { get; set; } = 5;

        [BsonElement("todayProgress")]
        public int TodayProgress { get; set; } = 0;

        [BsonElement("profilePicture")]
        public string ProfilePicture { get; set; } = string.Empty;

        [BsonElement("isGuest")]
        public bool IsGuest { get; set; } = false;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;
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
