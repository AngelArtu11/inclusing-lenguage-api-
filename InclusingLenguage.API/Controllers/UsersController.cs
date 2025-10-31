using Microsoft.AspNetCore.Mvc;
using InclusingLenguage.API.Models;
using InclusingLenguage.API.Services;
using MongoDB.Driver;

namespace InclusingLenguage.API.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
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

        [HttpGet("{usuarioID}")]
        public async Task<ActionResult<UserProfile>> GetByUsuarioID(string usuarioID)
        {
            var user = await _mongoDBService.Users
                .Find(u => u.UsuarioID == usuarioID)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            user.Pass = string.Empty;
            user.PasswordHash = null;
            return Ok(user);
        }

        [HttpPut("{usuarioID}")]
        public async Task<ActionResult<UserProfile>> Update(string usuarioID, [FromBody] UserProfile updatedUser)
        {
            var user = await _mongoDBService.Users
                .Find(u => u.UsuarioID == usuarioID)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            // Actualizar solo los campos permitidos de la colección Usuarios
            var update = Builders<UserProfile>.Update
                .Set(u => u.Nombre, updatedUser.Nombre)
                .Set(u => u.Correo, updatedUser.Correo);

            await _mongoDBService.Users.UpdateOneAsync(u => u.UsuarioID == usuarioID, update);

            var result = await _mongoDBService.Users
                .Find(u => u.UsuarioID == usuarioID)
                .FirstOrDefaultAsync();

            result.Pass = string.Empty;
            result.PasswordHash = null;
            return Ok(result);
        }

        [HttpDelete("{usuarioID}")]
        public async Task<IActionResult> Delete(string usuarioID)
        {
            var result = await _mongoDBService.Users.DeleteOneAsync(u => u.UsuarioID == usuarioID);

            if (result.DeletedCount == 0)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new { message = "Usuario eliminado exitosamente" });
        }

        [HttpPost("{usuarioID}/progress")]
        public async Task<ActionResult> UpdateProgress(string usuarioID, [FromBody] LessonProgressUpdate progressUpdate)
        {
            // NOTA: Este endpoint está deprecado. Usar /api/progresion en su lugar.
            // Se mantiene solo para compatibilidad con versiones antiguas.
            var user = await _mongoDBService.Users
                .Find(u => u.UsuarioID == usuarioID)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            // Delegar al endpoint correcto de Progresion
            return Ok(new {
                message = "Usar /api/progresion para actualizar el progreso",
                deprecated = true
            });
        }
    }

    public class LessonProgressUpdate
    {
        public string LessonId { get; set; } = string.Empty;
        public double Progress { get; set; }
        public int ExperienceGained { get; set; }
    }
}
