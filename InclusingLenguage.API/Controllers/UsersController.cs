using Microsoft.AspNetCore.Mvc;
using InclusingLenguage.API.Models;
using InclusingLenguage.API.Services;
using MongoDB.Driver;

namespace InclusingLenguage.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMongoDBService _mongoDBService;

        public UsersController(IMongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserProfile>>> GetAll()
        {
            var users = await _mongoDBService.Users.Find(_ => true).ToListAsync();

            // No retornar password hashes
            foreach (var user in users)
            {
                user.Pass = string.Empty;
                user.PasswordHash = null;
            }

            return Ok(users);
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<UserProfile>> GetByEmail(string email)
        {
            var user = await _mongoDBService.Users
                .Find(u => u.Correo == email)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            user.Pass = string.Empty;
            user.PasswordHash = null;
            return Ok(user);
        }

        [HttpPut("{email}")]
        public async Task<ActionResult<UserProfile>> Update(string email, [FromBody] UserProfile updatedUser)
        {
            var user = await _mongoDBService.Users
                .Find(u => u.Email == email)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            // Actualizar solo los campos permitidos
            var update = Builders<UserProfile>.Update
                .Set(u => u.FirstName, updatedUser.FirstName)
                .Set(u => u.LastName, updatedUser.LastName)
                .Set(u => u.Name, $"{updatedUser.FirstName} {updatedUser.LastName}")
                .Set(u => u.Level, updatedUser.Level)
                .Set(u => u.Experience, updatedUser.Experience)
                .Set(u => u.Streak, updatedUser.Streak)
                .Set(u => u.CompletedLessons, updatedUser.CompletedLessons)
                .Set(u => u.LessonProgress, updatedUser.LessonProgress)
                .Set(u => u.DailyGoal, updatedUser.DailyGoal)
                .Set(u => u.TodayProgress, updatedUser.TodayProgress)
                .Set(u => u.ProfilePicture, updatedUser.ProfilePicture);

            await _mongoDBService.Users.UpdateOneAsync(u => u.Email == email, update);

            var result = await _mongoDBService.Users
                .Find(u => u.Email == email)
                .FirstOrDefaultAsync();

            result.Pass = string.Empty;
            return Ok(result);
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> Delete(string email)
        {
            var result = await _mongoDBService.Users.DeleteOneAsync(u => u.Email == email);

            if (result.DeletedCount == 0)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new { message = "Usuario eliminado exitosamente" });
        }

        [HttpPost("{email}/progress")]
        public async Task<ActionResult> UpdateProgress(string email, [FromBody] LessonProgressUpdate progressUpdate)
        {
            var user = await _mongoDBService.Users
                .Find(u => u.Email == email)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            var update = Builders<UserProfile>.Update
                .Set($"lessonProgress.{progressUpdate.LessonId}", progressUpdate.Progress)
                .Set(u => u.Experience, user.Experience + progressUpdate.ExperienceGained)
                .Set(u => u.TodayProgress, user.TodayProgress + 1);

            // Si completó la lección, agregarla a completedLessons
            if (progressUpdate.Progress >= 100 && !user.CompletedLessons.Contains(progressUpdate.LessonId))
            {
                update = update.AddToSet(u => u.CompletedLessons, progressUpdate.LessonId);
            }

            await _mongoDBService.Users.UpdateOneAsync(u => u.Email == email, update);

            return Ok(new { message = "Progreso actualizado" });
        }
    }

    public class LessonProgressUpdate
    {
        public string LessonId { get; set; } = string.Empty;
        public double Progress { get; set; }
        public int ExperienceGained { get; set; }
    }
}
