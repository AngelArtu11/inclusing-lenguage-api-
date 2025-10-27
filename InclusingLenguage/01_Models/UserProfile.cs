using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InclusingLenguage._01_Models
{
    public class UserProfile
    {
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int Streak { get; set; } = 0;
        public DateTime LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> CompletedLessons { get; set; } = new List<string>();
        public Dictionary<string, double> LessonProgress { get; set; } = new Dictionary<string, double>();
        public int DailyGoal { get; set; } = 5; // Lecciones por día
        public int TodayProgress { get; set; } = 0;
        public string ProfilePicture { get; set; } = "";
        public bool IsGuest { get; set; } = false;
    }

    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public string ErrorCode { get; set; } = "";
        public UserProfile? UserProfile { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public bool RememberMe { get; set; } = false;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
    }
}
