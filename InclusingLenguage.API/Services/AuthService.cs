using MongoDB.Driver;
using InclusingLenguage.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace InclusingLenguage.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly IMongoDBService _mongoDBService;

        public AuthService(IMongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Verificar si el email ya existe
                var existingUser = await _mongoDBService.Users
                    .Find(u => u.Correo == request.Email)
                    .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    return new AuthResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "El email ya está registrado"
                    };
                }

                // Generar usuarioID único
                var usuarioID = $"u{DateTime.UtcNow.Ticks}";

                // Crear nuevo usuario con estructura correcta
                var newUser = new UserProfile
                {
                    UsuarioID = usuarioID,
                    Correo = request.Email,
                    Nombre = !string.IsNullOrWhiteSpace(request.Username)
                        ? request.Username
                        : $"{request.FirstName} {request.LastName}".Trim(),
                    FechaRegistro = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    Pass = HashPassword(request.Password)
                };

                await _mongoDBService.Users.InsertOneAsync(newUser);

                // Crear progresión inicial para el usuario
                var progresion = new Progresion
                {
                    UsuarioID = usuarioID,
                    NivelActual = 0,
                    NivelesCompletados = new List<int>(),
                    Intentos = new List<Intento>(),
                    Estadisticas = new Estadisticas
                    {
                        TiempoJugadoMin = 0,
                        TotalIntentos = 0,
                        TotalExitos = 0
                    }
                };

                await _mongoDBService.Progresion.InsertOneAsync(progresion);

                // No retornar el password hash
                newUser.Pass = string.Empty;

                return new AuthResponse
                {
                    IsSuccess = true,
                    UsuarioID = usuarioID,
                    UserProfile = newUser,
                    Token = GenerateToken(newUser.Correo)
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error al registrar: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _mongoDBService.Users
                    .Find(u => u.Correo == request.Email)
                    .FirstOrDefaultAsync();

                // Obtener el hash correcto (soportar tanto "pass" como "passwordHash")
                var passwordHash = !string.IsNullOrEmpty(user?.Pass) ? user.Pass : user?.PasswordHash ?? "";

                if (user == null || !VerifyPassword(request.Password, passwordHash))
                {
                    return new AuthResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Email o contraseña incorrectos"
                    };
                }

                // No retornar el hash
                user.Pass = string.Empty;
                user.PasswordHash = null;

                return new AuthResponse
                {
                    IsSuccess = true,
                    UsuarioID = user.UsuarioID,
                    UserProfile = user,
                    Token = GenerateToken(user.Correo)
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error al iniciar sesión: {ex.Message}"
                };
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }

        private string GenerateToken(string email)
        {
            // Por ahora un token simple - en producción usar JWT
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{DateTime.UtcNow.Ticks}"));
        }
    }
}
