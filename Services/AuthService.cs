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
                    .Find(u => u.Email == request.Email)
                    .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    return new AuthResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "El email ya está registrado"
                    };
                }

                // Crear nuevo usuario
                var newUser = new UserProfile
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    // Si se proporciona Username, usarlo; de lo contrario, usar FirstName LastName
                    Name = !string.IsNullOrWhiteSpace(request.Username)
                        ? request.Username
                        : $"{request.FirstName} {request.LastName}".Trim(),
                    PasswordHash = HashPassword(request.Password),
                    CreatedAt = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    Level = 1,
                    Experience = 0,
                    Streak = 0
                };

                await _mongoDBService.Users.InsertOneAsync(newUser);

                // No retornar el password hash
                newUser.PasswordHash = string.Empty;

                return new AuthResponse
                {
                    IsSuccess = true,
                    UserProfile = newUser,
                    Token = GenerateToken(newUser.Email)
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
                    .Find(u => u.Email == request.Email)
                    .FirstOrDefaultAsync();

                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                {
                    return new AuthResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Email o contraseña incorrectos"
                    };
                }

                // Actualizar último login
                var update = Builders<UserProfile>.Update.Set(u => u.LastLogin, DateTime.UtcNow);
                await _mongoDBService.Users.UpdateOneAsync(u => u.Email == request.Email, update);

                user.LastLogin = DateTime.UtcNow;
                user.PasswordHash = string.Empty; // No retornar el hash

                return new AuthResponse
                {
                    IsSuccess = true,
                    UserProfile = user,
                    Token = GenerateToken(user.Email)
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
