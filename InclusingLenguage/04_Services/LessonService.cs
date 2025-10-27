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
    public interface ILessonService
    {
        Task<List<Lesson>> GetAllLessonsAsync();
        Task<List<Lesson>> GetLessonsByCategoryAsync(string category);
        Task<Lesson> GetLessonByIdAsync(int id);
        Task<Lesson> GetNextIncompleteLessonAsync(string category);
        Task<bool> CompleteLessonAsync(int lessonId, int score, string userEmail);
        Task<LessonProgress> GetLessonProgressAsync(int lessonId, string userEmail);
        Task<int> GetCompletedLessonsCountAsync(string category, string userEmail);
    }

    public class LessonService : ILessonService
    {
        private static List<Lesson> _lessons = new List<Lesson>();
        private readonly IMongoCollection<LessonRecordDocument> _lessonRecordsCollection;
        private readonly IMongoCollection<UserDocument> _usersCollection;
        private readonly IMongoCollection<DailyActivityDocument> _activityCollection;

        public LessonService()
        {
            try
            {
                var mongoDBService = new MongoDBService();
                _lessonRecordsCollection = mongoDBService.GetCollection<LessonRecordDocument>("lesson_records");
                _usersCollection = mongoDBService.GetCollection<UserDocument>("users");
                _activityCollection = mongoDBService.GetCollection<DailyActivityDocument>("daily_activity");

                if (_lessons.Count == 0)
                {
                    InitializeLessons();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inicializando LessonService: {ex.Message}");
            }
        }

        private void InitializeLessons()
        {
            var alphabetData = new[]
            {
                new { Letter = "A", Desc = "Cierra el puño con el pulgar al lado", Emoji = "✊" },
                new { Letter = "B", Desc = "Mano abierta con dedos juntos, pulgar hacia la palma", Emoji = "🖐️" },
                new { Letter = "C", Desc = "Forma una C con la mano", Emoji = "👌" },
                new { Letter = "D", Desc = "Dedo índice arriba, otros dedos tocando el pulgar", Emoji = "☝️" },
                new { Letter = "E", Desc = "Dedos doblados hacia la palma", Emoji = "✊" },
                new { Letter = "F", Desc = "Índice y pulgar formando un círculo", Emoji = "👌" },
                new { Letter = "G", Desc = "Índice y pulgar horizontal", Emoji = "👉" },
                new { Letter = "H", Desc = "Índice y medio horizontal", Emoji = "✌️" },
                new { Letter = "I", Desc = "Solo meñique extendido", Emoji = "🤙" },
                new { Letter = "J", Desc = "Meñique hace una J en el aire", Emoji = "🤙" },
                new { Letter = "K", Desc = "Índice y medio en V, pulgar en medio", Emoji = "✌️" },
                new { Letter = "L", Desc = "L con índice y pulgar", Emoji = "🤟" },
                new { Letter = "M", Desc = "Pulgar bajo tres dedos", Emoji = "✊" },
                new { Letter = "N", Desc = "Pulgar bajo dos dedos", Emoji = "✊" },
                new { Letter = "Ñ", Desc = "Como N con movimiento", Emoji = "✊" },
                new { Letter = "O", Desc = "Dedos formando círculo", Emoji = "👌" },
                new { Letter = "P", Desc = "Como K pero hacia abajo", Emoji = "✌️" },
                new { Letter = "Q", Desc = "Como G pero hacia abajo", Emoji = "👉" },
                new { Letter = "R", Desc = "Índice y medio cruzados", Emoji = "🤞" },
                new { Letter = "S", Desc = "Puño cerrado con pulgar al frente", Emoji = "👊" },
                new { Letter = "T", Desc = "Pulgar entre índice y medio", Emoji = "👊" },
                new { Letter = "U", Desc = "Índice y medio juntos hacia arriba", Emoji = "✌️" },
                new { Letter = "V", Desc = "Índice y medio en V", Emoji = "✌️" },
                new { Letter = "W", Desc = "Índice, medio y anular en W", Emoji = "🖖" },
                new { Letter = "X", Desc = "Índice doblado en forma de gancho", Emoji = "☝️" },
                new { Letter = "Y", Desc = "Pulgar y meñique extendidos", Emoji = "🤙" },
                new { Letter = "Z", Desc = "Índice traza una Z en el aire", Emoji = "☝️" }
            };

            for (int i = 0; i < alphabetData.Length; i++)
            {
                var data = alphabetData[i];
                var lesson = new Lesson
                {
                    Id = i + 1,
                    Title = $"Letra {data.Letter}",
                    Category = "Alphabet",
                    Letter = data.Letter,
                    Description = data.Desc,
                    Order = i + 1,
                    ExperiencePoints = 15,
                    Difficulty = DifficultyLevel.Basic,
                    EstimatedMinutes = 5,
                    IsLocked = false,
                    IsCompleted = false,
                    ImageUrl = data.Emoji,
                    LearningTips = new List<string>
                    {
                        "Practica frente a un espejo",
                        "Observa la posición de cada dedo",
                        "Repite varias veces hasta sentirte cómodo"
                    },
                    Exercises = GenerateExercisesForLetter(data.Letter, data.Desc, i)
                };

                _lessons.Add(lesson);
            }
        }

        private List<Exercise> GenerateExercisesForLetter(string letter, string description, int index)
        {
            var allLetters = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                                    "K", "L", "M", "N", "Ñ", "O", "P", "Q", "R", "S",
                                    "T", "U", "V", "W", "X", "Y", "Z" };

            var wrongOptions = allLetters.Where(l => l != letter).OrderBy(x => Guid.NewGuid()).Take(3).ToList();

            return new List<Exercise>
            {
                new Exercise
                {
                    Id = 1,
                    Type = ExerciseType.Practice,
                    Question = $"Aprende la seña de la letra {letter}",
                    CorrectAnswer = letter,
                    HintText = description,
                    Points = 5
                },
                new Exercise
                {
                    Id = 2,
                    Type = ExerciseType.SignRecognition,
                    Question = "¿Qué letra representa esta seña?",
                    CorrectAnswer = letter,
                    Options = new List<string> { letter }.Concat(wrongOptions).OrderBy(x => Guid.NewGuid()).ToList(),
                    HintText = $"Recuerda: {description}",
                    Points = 10
                },
                new Exercise
                {
                    Id = 3,
                    Type = ExerciseType.MultipleChoice,
                    Question = $"¿Cómo se hace la seña de la letra {letter}?",
                    CorrectAnswer = description,
                    Options = new List<string>
                    {
                        description,
                        "Todos los dedos extendidos hacia arriba",
                        "Puño cerrado completamente",
                        "Dedos formando un círculo"
                    }.OrderBy(x => Guid.NewGuid()).ToList(),
                    HintText = "Piensa en la posición de los dedos",
                    Points = 10
                }
            };
        }

        public async Task<List<Lesson>> GetAllLessonsAsync()
        {
            await Task.Delay(100);
            return _lessons;
        }

        public async Task<List<Lesson>> GetLessonsByCategoryAsync(string category)
        {
            await Task.Delay(100);
            return _lessons.Where(l => l.Category == category).OrderBy(l => l.Order).ToList();
        }

        public async Task<Lesson> GetLessonByIdAsync(int id)
        {
            await Task.Delay(100);
            return _lessons.FirstOrDefault(l => l.Id == id);
        }

        public async Task<Lesson> GetNextIncompleteLessonAsync(string category)
        {
            await Task.Delay(100);

            var nextLesson = _lessons
                .Where(l => l.Category == category && !l.IsCompleted)
                .OrderBy(l => l.Order)
                .FirstOrDefault();

            if (nextLesson == null)
            {
                nextLesson = _lessons.FirstOrDefault(l => l.Category == category);
            }

            return nextLesson;
        }

        public async Task<bool> CompleteLessonAsync(int lessonId, int score, string userEmail)
        {
            try
            {
                var lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);
                if (lesson == null) return false;

                var email = userEmail.ToLower().Trim();
                var totalPoints = lesson.Exercises.Sum(e => e.Points);

                // Guardar registro de lección en MongoDB
                var lessonRecord = new LessonRecordDocument
                {
                    Email = email,
                    LessonId = lessonId,
                    LessonName = lesson.Title,
                    Category = lesson.Category,
                    Score = score,
                    TotalPoints = totalPoints,
                    TimeSpentMinutes = lesson.EstimatedMinutes,
                    IsPerfect = score >= 90,
                    CompletedAt = DateTime.UtcNow,
                    ExercisesCompleted = lesson.Exercises.Count
                };

                await _lessonRecordsCollection.InsertOneAsync(lessonRecord);

                // Actualizar usuario
                var userUpdate = Builders<UserDocument>.Update
                    .Set(u => u.LastActiveDate, DateTime.UtcNow.Date)
                    .Inc(u => u.Experience, lesson.ExperiencePoints)
                    .Inc(u => u.TodayProgress, 1);

                await _usersCollection.UpdateOneAsync(u => u.Email == email, userUpdate);

                // Registrar actividad diaria
                var today = DateTime.UtcNow.Date;
                var dailyActivity = await _activityCollection
                    .Find(a => a.Email == email && a.Date == today)
                    .FirstOrDefaultAsync();

                if (dailyActivity == null)
                {
                    dailyActivity = new DailyActivityDocument
                    {
                        Email = email,
                        Date = today,
                        LessonsCompleted = 1,
                        XPGained = lesson.ExperiencePoints,
                        MinutesPracticed = lesson.EstimatedMinutes,
                        MetDailyGoal = false
                    };
                    await _activityCollection.InsertOneAsync(dailyActivity);
                }
                else
                {
                    var activityUpdate = Builders<DailyActivityDocument>.Update
                        .Inc(a => a.LessonsCompleted, 1)
                        .Inc(a => a.XPGained, lesson.ExperiencePoints)
                        .Inc(a => a.MinutesPracticed, lesson.EstimatedMinutes);

                    await _activityCollection.UpdateOneAsync(a => a.Email == email && a.Date == today, activityUpdate);
                }

                // Marcar lección como completada localmente
                lesson.IsCompleted = true;

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error completando lección: {ex.Message}");
                return false;
            }
        }

        public async Task<LessonProgress> GetLessonProgressAsync(int lessonId, string userEmail)
        {
            try
            {
                await Task.Delay(100);
                var email = userEmail.ToLower().Trim();

                var record = await _lessonRecordsCollection
                    .Find(r => r.LessonId == lessonId && r.Email == email)
                    .FirstOrDefaultAsync();

                if (record != null)
                {
                    return new LessonProgress
                    {
                        LessonId = lessonId,
                        ExercisesCompleted = record.ExercisesCompleted,
                        TotalExercises = record.ExercisesCompleted,
                        ProgressPercentage = 100,
                        Score = record.Score,
                        CompletedAt = record.CompletedAt,
                        IsPerfect = record.IsPerfect
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo progreso: {ex.Message}");
                return null;
            }
        }

        public async Task<int> GetCompletedLessonsCountAsync(string category, string userEmail)
        {
            try
            {
                var email = userEmail.ToLower().Trim();

                var count = await _lessonRecordsCollection
                    .CountDocumentsAsync(r => r.Category == category && r.Email == email);

                return (int)count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error contando lecciones: {ex.Message}");
                return 0;
            }
        }
    }
}
