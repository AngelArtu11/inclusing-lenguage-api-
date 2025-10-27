using InclusingLenguage._01_Models;
using InclusingLenguage._04_Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InclusingLenguage._04_Services
{
    public interface IProfileService
    {
        Task<UserProfileExtended> GetUserProfileExtendedAsync(string email);
        Task<UserStats> GetUserStatsAsync(string email);
        Task<List<Achievement>> GetUserAchievementsAsync(string email);
        Task<bool> UpdateUserProfileAsync(UserProfile user);
        Task<bool> UpdateUserSettingsAsync(string email, UserSettings settings);
        Task<bool> UpdateDailyStreakAsync(string email);
        Task<int> GetCurrentStreakAsync(string email);
        Task<List<DailyActivity>> GetActivityHistoryAsync(string email, int days = 30);
        Task<bool> CheckAndAwardAchievementsAsync(string email, UserStats stats);
        Task<List<Badge>> GetUserBadgesAsync(string email);
        Task<int> GetTotalLessonsCompletedAsync(string email);
        Task<int> GetAverageScoreAsync(string email);
        Task<Dictionary<string, int>> GetCategoryProgressAsync(string email);
        Task<List<LessonRecord>> GetLessonHistoryAsync(string email, int limit = 10);
        Task<bool> SaveLessonRecordAsync(string email, LessonRecord record);
    }

    public class ProfileService : IProfileService
    {
        private readonly IMongoCollection<UserDocument> _usersCollection;
        private readonly IMongoCollection<LessonRecordDocument> _lessonRecordsCollection;
        private readonly IMongoCollection<DailyActivityDocument> _activityCollection;
        private readonly IMongoCollection<BadgeDocument> _badgesCollection;

        public ProfileService()
        {
            try
            {
                var mongoDBService = new MongoDBService();
                _usersCollection = mongoDBService.GetCollection<UserDocument>("users");
                _lessonRecordsCollection = mongoDBService.GetCollection<LessonRecordDocument>("lesson_records");
                _activityCollection = mongoDBService.GetCollection<DailyActivityDocument>("daily_activity");
                _badgesCollection = mongoDBService.GetCollection<BadgeDocument>("badges");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inicializando ProfileService: {ex.Message}");
            }
        }

        public async Task<UserProfileExtended> GetUserProfileExtendedAsync(string email)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();

                // Obtener usuario
                var userDoc = await _usersCollection.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
                if (userDoc == null) return null;

                // Obtener estadísticas
                var stats = await GetUserStatsAsync(normalizedEmail);

                // Obtener badges
                var badges = await GetUserBadgesAsync(normalizedEmail);

                // Obtener historial de lecciones
                var lessonRecords = await GetLessonHistoryAsync(normalizedEmail, 10);

                var userProfile = new UserProfile
                {
                    Email = userDoc.Email,
                    Name = $"{userDoc.FirstName} {userDoc.LastName}".Trim(),
                    FirstName = userDoc.FirstName,
                    LastName = userDoc.LastName,
                    Level = userDoc.Level,
                    Experience = userDoc.Experience,
                    Streak = userDoc.CurrentStreak,
                    LastLogin = userDoc.LastLogin,
                    CreatedAt = userDoc.CreatedAt,
                    DailyGoal = userDoc.DailyGoal,
                    TodayProgress = userDoc.TodayProgress
                };

                return new UserProfileExtended
                {
                    BasicInfo = userProfile,
                    Statistics = stats,
                    Badges = badges,
                    LessonRecords = lessonRecords
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo perfil extendido: {ex.Message}");
                return null;
            }
        }

        public async Task<UserStats> GetUserStatsAsync(string email)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();

                var userDoc = await _usersCollection.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
                if (userDoc == null) return null;

                // Contar lecciones completadas por categoría
                var alphabetCount = await _lessonRecordsCollection.CountDocumentsAsync(
                    r => r.Email == normalizedEmail && r.Category == "Alphabet");
                var numbersCount = await _lessonRecordsCollection.CountDocumentsAsync(
                    r => r.Email == normalizedEmail && r.Category == "Numbers");
                var wordsCount = await _lessonRecordsCollection.CountDocumentsAsync(
                    r => r.Email == normalizedEmail && r.Category == "Words");

                // Calcular promedio de puntuaciones
                var lessons = await _lessonRecordsCollection
                    .Find(r => r.Email == normalizedEmail)
                    .ToListAsync();

                int averageScore = lessons.Any()
                    ? (int)lessons.Average(l => l.Score)
                    : 0;

                // Contar sesiones de práctica (días únicos)
                var practiseDays = await _activityCollection
                    .Find(a => a.Email == normalizedEmail)
                    .ToListAsync();

                int totalMinutes = practiseDays.Sum(d => d.MinutesPracticed);

                return new UserStats
                {
                    TotalLessonsCompleted = lessons.Count,
                    TotalExperienceGained = userDoc.Experience,
                    CurrentStreak = userDoc.CurrentStreak,
                    LongestStreak = userDoc.LongestStreak,
                    LastActiveDate = userDoc.LastActiveDate,
                    AverageScore = averageScore,
                    CategoryProgress = new Dictionary<string, int>
                    {
                        { "Alphabet", (int)alphabetCount },
                        { "Numbers", (int)numbersCount },
                        { "Words", (int)wordsCount }
                    },
                    MemberSince = userDoc.CreatedAt,
                    TotalPracticeSessions = practiseDays.Count,
                    TotalMinutesPracticed = totalMinutes,
                    Achievements = await GetUserAchievementsAsync(normalizedEmail)
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo estadísticas: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Achievement>> GetUserAchievementsAsync(string email)
        {
            // TODO: Implementar logros basados en hitos
            return await Task.FromResult(new List<Achievement>());
        }

        public async Task<bool> UpdateUserProfileAsync(UserProfile user)
        {
            try
            {
                var normalizedEmail = user.Email.ToLower().Trim();

                var update = Builders<UserDocument>.Update
                    .Set(u => u.FirstName, user.FirstName)
                    .Set(u => u.LastName, user.LastName);

                var result = await _usersCollection.UpdateOneAsync(
                    u => u.Email == normalizedEmail,
                    update);

                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error actualizando perfil: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUserSettingsAsync(string email, UserSettings settings)
        {
            try
            {
                // TODO: Crear colección de configuración si es necesario
                await Task.Delay(100);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error actualizando configuración: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateDailyStreakAsync(string email)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();
                var userDoc = await _usersCollection.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();

                if (userDoc == null) return false;

                var daysSinceLastActive = (DateTime.UtcNow.Date - userDoc.LastActiveDate.Date).Days;

                if (daysSinceLastActive == 0)
                {
                    return true; // Mismo día
                }
                else if (daysSinceLastActive == 1)
                {
                    // Día consecutivo
                    var update = Builders<UserDocument>.Update
                        .Inc(u => u.CurrentStreak, 1)
                        .Set(u => u.LastActiveDate, DateTime.UtcNow.Date);

                    await _usersCollection.UpdateOneAsync(u => u.Email == normalizedEmail, update);

                    // Actualizar racha más larga
                    if (userDoc.CurrentStreak + 1 > userDoc.LongestStreak)
                    {
                        var updateLongest = Builders<UserDocument>.Update
                            .Set(u => u.LongestStreak, userDoc.CurrentStreak + 1);
                        await _usersCollection.UpdateOneAsync(u => u.Email == normalizedEmail, updateLongest);
                    }
                }
                else
                {
                    // Racha rota
                    var update = Builders<UserDocument>.Update
                        .Set(u => u.CurrentStreak, 1)
                        .Set(u => u.LastActiveDate, DateTime.UtcNow.Date);

                    await _usersCollection.UpdateOneAsync(u => u.Email == normalizedEmail, update);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error actualizando racha: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetCurrentStreakAsync(string email)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();
                var userDoc = await _usersCollection.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
                return userDoc?.CurrentStreak ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<List<DailyActivity>> GetActivityHistoryAsync(string email, int days = 30)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();
                var startDate = DateTime.UtcNow.Date.AddDays(-days);

                var activities = await _activityCollection
                    .Find(a => a.Email == normalizedEmail && a.Date >= startDate)
                    .SortByDescending(a => a.Date)
                    .ToListAsync();

                return activities.Select(a => new DailyActivity
                {
                    Date = a.Date,
                    LessonsCompleted = a.LessonsCompleted,
                    XPGained = a.XPGained,
                    MinutesPracticed = a.MinutesPracticed,
                    MetDailyGoal = a.MetDailyGoal
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo historial: {ex.Message}");
                return new List<DailyActivity>();
            }
        }

        public async Task<bool> CheckAndAwardAchievementsAsync(string email, UserStats stats)
        {
            try
            {
                // TODO: Implementar lógica de logros
                await Task.Delay(100);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error verificando logros: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Badge>> GetUserBadgesAsync(string email)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();

                var badgeDocs = await _badgesCollection
                    .Find(b => b.Email == normalizedEmail)
                    .SortByDescending(b => b.EarnedAt)
                    .ToListAsync();

                return badgeDocs.Select(b => new Badge
                {
                    Id = b.Id.ToString().GetHashCode(),
                    Title = b.Title,
                    Icon = b.Icon,
                    EarnedAt = b.EarnedAt,
                    Reason = b.Reason
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo badges: {ex.Message}");
                return new List<Badge>();
            }
        }

        public async Task<int> GetTotalLessonsCompletedAsync(string email)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();
                var count = await _lessonRecordsCollection.CountDocumentsAsync(r => r.Email == normalizedEmail);
                return (int)count;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> GetAverageScoreAsync(string email)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();
                var lessons = await _lessonRecordsCollection
                    .Find(r => r.Email == normalizedEmail)
                    .ToListAsync();

                return lessons.Any() ? (int)lessons.Average(l => l.Score) : 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<Dictionary<string, int>> GetCategoryProgressAsync(string email)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();

                var alphabet = await _lessonRecordsCollection.CountDocumentsAsync(
                    r => r.Email == normalizedEmail && r.Category == "Alphabet");
                var numbers = await _lessonRecordsCollection.CountDocumentsAsync(
                    r => r.Email == normalizedEmail && r.Category == "Numbers");
                var words = await _lessonRecordsCollection.CountDocumentsAsync(
                    r => r.Email == normalizedEmail && r.Category == "Words");

                return new Dictionary<string, int>
                {
                    { "Alphabet", (int)alphabet },
                    { "Numbers", (int)numbers },
                    { "Words", (int)words }
                };
            }
            catch
            {
                return new Dictionary<string, int>();
            }
        }

        public async Task<List<LessonRecord>> GetLessonHistoryAsync(string email, int limit = 10)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();

                var lessonDocs = await _lessonRecordsCollection
                    .Find(r => r.Email == normalizedEmail)
                    .SortByDescending(r => r.CompletedAt)
                    .Limit(limit)
                    .ToListAsync();

                return lessonDocs.Select(r => new LessonRecord
                {
                    LessonId = r.LessonId,
                    LessonName = r.LessonName,
                    Score = r.Score,
                    CompletedAt = r.CompletedAt,
                    TimeSpentMinutes = r.TimeSpentMinutes,
                    IsPerfect = r.IsPerfect
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo historial de lecciones: {ex.Message}");
                return new List<LessonRecord>();
            }
        }

        public async Task<bool> SaveLessonRecordAsync(string email, LessonRecord record)
        {
            try
            {
                var normalizedEmail = email.ToLower().Trim();

                var lessonDoc = new LessonRecordDocument
                {
                    Email = normalizedEmail,
                    LessonId = record.LessonId,
                    LessonName = record.LessonName,
                    Category = "Alphabet", // Obtener de la lección real
                    Score = record.Score,
                    TimeSpentMinutes = record.TimeSpentMinutes,
                    IsPerfect = record.IsPerfect,
                    CompletedAt = record.CompletedAt,
                    ExercisesCompleted = 3 // Número de ejercicios por lección
                };

                await _lessonRecordsCollection.InsertOneAsync(lessonDoc);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error guardando registro de lección: {ex.Message}");
                return false;
            }
        }
    }
}
