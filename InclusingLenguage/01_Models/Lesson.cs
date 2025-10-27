using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InclusingLenguage._01_Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Category { get; set; } = ""; // "Alphabet", "Numbers", "Words"
        public string Letter { get; set; } = ""; // Para alfabeto: "A", "B", etc.
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string VideoUrl { get; set; } = "";
        public string GifUrl { get; set; } = ""; // Animación de la seña
        public int Order { get; set; } // Orden en la secuencia
        public int ExperiencePoints { get; set; } = 10;
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Basic;
        public bool IsCompleted { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
        public List<string> LearningTips { get; set; } = new List<string>();
        public int EstimatedMinutes { get; set; } = 5;
    }

    public enum DifficultyLevel
    {
        Basic,
        Intermediate,
        Advanced
    }

    public class Exercise
    {
        public int Id { get; set; }
        public ExerciseType Type { get; set; }
        public string Question { get; set; } = "";
        public string CorrectAnswer { get; set; } = "";
        public List<string> Options { get; set; } = new List<string>(); // Para opción múltiple
        public string ImageUrl { get; set; } = "";
        public string HintText { get; set; } = "";
        public int Points { get; set; } = 5;
    }

    public enum ExerciseType
    {
        MultipleChoice,     // Elegir la respuesta correcta
        SignRecognition,    // Reconocer la seña mostrada
        Practice,           // Ver y aprender la seña
        Matching,           // Emparejar letra con seña
        TrueFalse          // Verdadero o falso
    }

    public class LessonProgress
    {
        public int LessonId { get; set; }
        public int ExercisesCompleted { get; set; }
        public int TotalExercises { get; set; }
        public double ProgressPercentage { get; set; }
        public int Score { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsPerfect { get; set; } // Si obtuvo 100%
    }
}
