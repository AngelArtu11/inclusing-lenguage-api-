using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InclusingLenguage._04_Services
{
    public class UserDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("password")]
        public string PasswordHash { get; set; }

        [BsonElement("level")]
        public int Level { get; set; } = 1;

        [BsonElement("experience")]
        public int Experience { get; set; } = 0;

        [BsonElement("currentStreak")]
        public int CurrentStreak { get; set; } = 0;

        [BsonElement("longestStreak")]
        public int LongestStreak { get; set; } = 0;

        [BsonElement("dailyGoal")]
        public int DailyGoal { get; set; } = 5;

        [BsonElement("todayProgress")]
        public int TodayProgress { get; set; } = 0;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("lastLogin")]
        public DateTime LastLogin { get; set; }

        [BsonElement("lastActiveDate")]
        public DateTime LastActiveDate { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }

    public class LessonRecordDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("lessonId")]
        public int LessonId { get; set; }

        [BsonElement("lessonName")]
        public string LessonName { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("score")]
        public int Score { get; set; }

        [BsonElement("totalPoints")]
        public int TotalPoints { get; set; }

        [BsonElement("timeSpentMinutes")]
        public int TimeSpentMinutes { get; set; }

        [BsonElement("isPerfect")]
        public bool IsPerfect { get; set; }

        [BsonElement("completedAt")]
        public DateTime CompletedAt { get; set; }

        [BsonElement("exercisesCompleted")]
        public int ExercisesCompleted { get; set; }
    }

    public class DailyActivityDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("lessonsCompleted")]
        public int LessonsCompleted { get; set; }

        [BsonElement("xpGained")]
        public int XPGained { get; set; }

        [BsonElement("minutesPracticed")]
        public int MinutesPracticed { get; set; }

        [BsonElement("metDailyGoal")]
        public bool MetDailyGoal { get; set; }
    }

    public class BadgeDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("icon")]
        public string Icon { get; set; }

        [BsonElement("earnedAt")]
        public DateTime EarnedAt { get; set; }

        [BsonElement("reason")]
        public string Reason { get; set; }
    }
}