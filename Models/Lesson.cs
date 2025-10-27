using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InclusingLenguage.API.Models
{
    public class Lesson
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("lessonId")]
        public int LessonId { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("letter")]
        public string Letter { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [BsonElement("videoUrl")]
        public string VideoUrl { get; set; } = string.Empty;

        [BsonElement("gifUrl")]
        public string GifUrl { get; set; } = string.Empty;

        [BsonElement("order")]
        public int Order { get; set; }

        [BsonElement("experiencePoints")]
        public int ExperiencePoints { get; set; } = 10;

        [BsonElement("difficulty")]
        public string Difficulty { get; set; } = "Basic";

        [BsonElement("exercises")]
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();

        [BsonElement("learningTips")]
        public List<string> LearningTips { get; set; } = new List<string>();

        [BsonElement("estimatedMinutes")]
        public int EstimatedMinutes { get; set; } = 5;
    }

    public class Exercise
    {
        [BsonElement("id")]
        public int Id { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("question")]
        public string Question { get; set; } = string.Empty;

        [BsonElement("correctAnswer")]
        public string CorrectAnswer { get; set; } = string.Empty;

        [BsonElement("options")]
        public List<string> Options { get; set; } = new List<string>();

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [BsonElement("hintText")]
        public string HintText { get; set; } = string.Empty;

        [BsonElement("points")]
        public int Points { get; set; } = 5;
    }
}
