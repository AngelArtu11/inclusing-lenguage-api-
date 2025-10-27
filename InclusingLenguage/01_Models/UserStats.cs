using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InclusingLenguage._01_Models
{

    public class UserStats
    {
        public int TotalLessonsCompleted { get; set; } = 0;
        public int TotalExperienceGained { get; set; } = 0;
        public int CurrentStreak { get; set; } = 0;
        public int LongestStreak { get; set; } = 0;
        public DateTime LastActiveDate { get; set; }
        public int AverageScore { get; set; } = 0;
        public Dictionary<string, int> CategoryProgress { get; set; } = new Dictionary<string, int>();
        public List<Achievement> Achievements { get; set; } = new List<Achievement>();
        public DateTime MemberSince { get; set; }
        public int TotalPracticeSessions { get; set; } = 0;
        public int TotalMinutesPracticed { get; set; } = 0;
    }

    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Icon { get; set; } = ""; // Emoji o URL
        public DateTime UnlockedAt { get; set; }
        public bool IsLocked { get; set; } = true;
        public string Requirement { get; set; } = ""; // Descripción del requisito
    }

    public class UserProfileExtended
    {
        public UserProfile BasicInfo { get; set; }
        public UserStats Statistics { get; set; }
        public List<Badge> Badges { get; set; } = new List<Badge>();
        public List<LessonRecord> LessonRecords { get; set; } = new List<LessonRecord>();
    }

    public class Badge
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "";
        public DateTime EarnedAt { get; set; }
        public string Reason { get; set; } = "";
    }

    public class LessonRecord
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; } = "";
        public int Score { get; set; }
        public DateTime CompletedAt { get; set; }
        public int TimeSpentMinutes { get; set; }
        public bool IsPerfect { get; set; } // 100% de aciertos
    }

    public class DailyActivity
    {
        public DateTime Date { get; set; }
        public int LessonsCompleted { get; set; }
        public int XPGained { get; set; }
        public int MinutesPracticed { get; set; }
        public bool MetDailyGoal { get; set; }
    }

    public class UserSettings
    {
        public int DailyGoalTarget { get; set; } = 5; // Lecciones por día
        public bool NotificationsEnabled { get; set; } = true;
        public string AppLanguage { get; set; } = "es";
        public bool SoundEnabled { get; set; } = true;
        public bool DarkMode { get; set; } = true;
        public DateTime ReminderTime { get; set; } // Hora para notificaciones
        public bool ShowHintsAutomatically { get; set; } = true;
    }
}
