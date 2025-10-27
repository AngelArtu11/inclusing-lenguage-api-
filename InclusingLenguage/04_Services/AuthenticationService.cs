using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InclusingLenguage._01_Models;
using InclusingLenguage._04_Services;
using System.Diagnostics;

namespace InclusingLenguage._04_Services
{
    public interface IAuthenticationService
    {
        Task<AuthResult> LoginAsync(LoginRequest request);
        Task<AuthResult> RegisterAsync(RegisterRequest request);
        Task<bool> LogoutAsync();
        Task<UserProfile> GetCurrentUserAsync();
        Task<bool> IsUserLoggedInAsync();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApiService _apiService;

        public AuthenticationService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorCode = "INVALID_INPUT",
                        ErrorMessage = "Por favor completa todos los campos"
                    };
                }

                // Llamar a la API
                var result = await _apiService.LoginAsync(request);

                if (result.IsSuccess && result.UserProfile != null)
                {
                    // Guardar token y email en almacenamiento seguro
                    await SecureStorage.SetAsync("user_token", result.Token);
                    await SecureStorage.SetAsync("user_email", result.UserProfile.Email);
                    await SecureStorage.SetAsync("is_guest", "false");
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en login: {ex.Message}");
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorCode = "SYSTEM_ERROR",
                    ErrorMessage = "Error de conexión con el servidor"
                };
            }
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.FirstName))
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorCode = "INVALID_INPUT",
                        ErrorMessage = "Por favor completa todos los campos requeridos"
                    };
                }

                if (request.Password != request.ConfirmPassword)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorCode = "PASSWORD_MISMATCH",
                        ErrorMessage = "Las contraseñas no coinciden"
                    };
                }

                if (request.Password.Length < 6)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorCode = "WEAK_PASSWORD",
                        ErrorMessage = "La contraseña debe tener al menos 6 caracteres"
                    };
                }

                // Llamar a la API
                var result = await _apiService.RegisterAsync(request);

                if (result.IsSuccess && result.UserProfile != null)
                {
                    // Guardar token y email en almacenamiento seguro
                    await SecureStorage.SetAsync("user_token", result.Token);
                    await SecureStorage.SetAsync("user_email", result.UserProfile.Email);
                    await SecureStorage.SetAsync("is_guest", "false");
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en registro: {ex.Message}");
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorCode = "SYSTEM_ERROR",
                    ErrorMessage = "Error de conexión con el servidor"
                };
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                await SecureStorage.SetAsync("user_token", "");
                await SecureStorage.SetAsync("user_email", "");
                await SecureStorage.SetAsync("is_guest", "false");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserProfile> GetCurrentUserAsync()
        {
            try
            {
                var userEmail = await SecureStorage.GetAsync("user_email");
                var isGuest = await SecureStorage.GetAsync("is_guest");

                if (isGuest == "true")
                {
                    return new UserProfile
                    {
                        Email = "guest@signlearn.com",
                        Name = "Usuario Invitado",
                        FirstName = "Usuario",
                        LastName = "Invitado",
                        IsGuest = true,
                        Level = 1,
                        Experience = 0,
                        Streak = 0
                    };
                }

                if (!string.IsNullOrEmpty(userEmail))
                {
                    // Obtener perfil desde la API
                    var userProfile = await _apiService.GetUserProfileAsync(userEmail);
                    return userProfile;
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo usuario actual: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> IsUserLoggedInAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("user_token");
                var email = await SecureStorage.GetAsync("user_email");
                return !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email);
            }
            catch
            {
                return false;
            }
        }
    }
}