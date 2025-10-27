using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using InclusingLenguage._01_Models;

namespace InclusingLenguage._04_Services
{
    public interface IApiService
    {
        Task<AuthResult> RegisterAsync(RegisterRequest request);
        Task<AuthResult> LoginAsync(LoginRequest request);
        Task<UserProfile?> GetUserProfileAsync(string email);
        Task<bool> UpdateUserProfileAsync(string email, UserProfile profile);
        Task<List<Lesson>> GetLessonsAsync();
        Task<List<Lesson>> GetLessonsByCategoryAsync(string category);
        Task<Lesson?> GetLessonByIdAsync(string id);
        Task<bool> UpdateLessonProgressAsync(string email, string lessonId, double progress, int experienceGained);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        // CONFIGURACIÓN: API desplegada en Railway (funciona desde cualquier lugar)
        private const string BaseUrl = "https://inclusing-lenguage-api-production.up.railway.app/api";

        // Para desarrollo local (descomenta si necesitas usar la API local):
        // private const string BaseUrl = "http://10.0.2.2:5256/api"; // Android Emulator
        // private const string BaseUrl = "http://192.168.1.68:5256/api"; // Dispositivo físico
        // private const string BaseUrl = "http://localhost:5256/api"; // iOS Simulator

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };

            // Configurar timeout más largo para desarrollo y debugging
            _httpClient.Timeout = TimeSpan.FromSeconds(120);

            // Log de configuración
            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine($"[ApiService] NUEVO CONSTRUCTOR LLAMADO");
            System.Diagnostics.Debug.WriteLine($"[ApiService] Configurado con BaseUrl: {BaseUrl}");
            System.Diagnostics.Debug.WriteLine($"[ApiService] HttpClient.BaseAddress: {_httpClient.BaseAddress}");
            System.Diagnostics.Debug.WriteLine($"[ApiService] Timeout: {_httpClient.Timeout.TotalSeconds} segundos");
            System.Diagnostics.Debug.WriteLine("========================================");
        }

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(120);
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService.RegisterAsync] Iniciando registro para: {request.Email}");
                System.Diagnostics.Debug.WriteLine($"[ApiService.RegisterAsync] FirstName: {request.FirstName}, LastName: {request.LastName}");
                System.Diagnostics.Debug.WriteLine($"[ApiService.RegisterAsync] URL completa: {_httpClient.BaseAddress}auth/register");
                System.Diagnostics.Debug.WriteLine($"[ApiService.RegisterAsync] BaseUrl configurado: {BaseUrl}");

                var response = await _httpClient.PostAsJsonAsync("/auth/register", request);

                System.Diagnostics.Debug.WriteLine($"[ApiService.RegisterAsync] Status Code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    System.Diagnostics.Debug.WriteLine($"[ApiService.RegisterAsync] Registro exitoso");
                    return new AuthResult
                    {
                        IsSuccess = authResponse?.IsSuccess ?? false,
                        Token = authResponse?.Token ?? "",
                        UserProfile = authResponse?.UserProfile,
                        ErrorMessage = authResponse?.ErrorMessage ?? ""
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[ApiService.RegisterAsync] Error HTTP Status {response.StatusCode}: {errorContent}");

                    // Intentar parsear como AuthResponse
                    try
                    {
                        var errorResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                        if (errorResponse != null)
                        {
                            return new AuthResult
                            {
                                IsSuccess = false,
                                ErrorMessage = errorResponse.ErrorMessage ?? $"Error {response.StatusCode}"
                            };
                        }
                    }
                    catch { }

                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = string.IsNullOrEmpty(errorContent)
                            ? $"Error en el registro (HTTP {response.StatusCode})"
                            : $"Error en el registro: {errorContent}"
                    };
                }
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService.RegisterAsync] Timeout: {ex.Message}");
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Timeout: La API no respondió a tiempo. Verifica que la API esté corriendo y que la URL sea correcta: {BaseUrl}"
                };
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService.RegisterAsync] Error HTTP: {ex.Message}");
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error de red: {ex.Message}. Verifica la conexión a {BaseUrl}"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService.RegisterAsync] Error general: {ex.GetType().Name} - {ex.Message}");
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error de conexión: {ex.Message}"
                };
            }
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService.LoginAsync] Iniciando login para: {request.Email}");
                System.Diagnostics.Debug.WriteLine($"[ApiService.LoginAsync] URL completa: {_httpClient.BaseAddress}auth/login");

                var response = await _httpClient.PostAsJsonAsync("/auth/login", request);

                System.Diagnostics.Debug.WriteLine($"[ApiService.LoginAsync] Status Code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    System.Diagnostics.Debug.WriteLine($"[ApiService.LoginAsync] Login exitoso");
                    return new AuthResult
                    {
                        IsSuccess = authResponse?.IsSuccess ?? false,
                        Token = authResponse?.Token ?? "",
                        UserProfile = authResponse?.UserProfile,
                        ErrorMessage = authResponse?.ErrorMessage ?? ""
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[ApiService.LoginAsync] Error HTTP: {errorContent}");
                    return new AuthResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"Error en el login: {errorContent}"
                    };
                }
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService.LoginAsync] Timeout: {ex.Message}");
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Timeout: La API no respondió a tiempo. Verifica que la API esté corriendo y que la URL sea correcta: {BaseUrl}"
                };
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService.LoginAsync] Error HTTP: {ex.Message}");
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error de red: {ex.Message}. Verifica la conexión a {BaseUrl}"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService.LoginAsync] Error general: {ex.GetType().Name} - {ex.Message}");
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error de conexión: {ex.Message}"
                };
            }
        }

        public async Task<UserProfile?> GetUserProfileAsync(string email)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserProfile>($"/users/{email}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo perfil: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(string email, UserProfile profile)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/users/{email}", profile);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error actualizando perfil: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Lesson>> GetLessonsAsync()
        {
            try
            {
                var lessons = await _httpClient.GetFromJsonAsync<List<Lesson>>("/lessons");
                return lessons ?? new List<Lesson>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo lecciones: {ex.Message}");
                return new List<Lesson>();
            }
        }

        public async Task<List<Lesson>> GetLessonsByCategoryAsync(string category)
        {
            try
            {
                var lessons = await _httpClient.GetFromJsonAsync<List<Lesson>>($"/lessons/category/{category}");
                return lessons ?? new List<Lesson>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo lecciones por categoría: {ex.Message}");
                return new List<Lesson>();
            }
        }

        public async Task<Lesson?> GetLessonByIdAsync(string id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Lesson>($"/lessons/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo lección: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateLessonProgressAsync(string email, string lessonId, double progress, int experienceGained)
        {
            try
            {
                var progressUpdate = new
                {
                    LessonId = lessonId,
                    Progress = progress,
                    ExperienceGained = experienceGained
                };

                var response = await _httpClient.PostAsJsonAsync($"/users/{email}/progress", progressUpdate);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error actualizando progreso: {ex.Message}");
                return false;
            }
        }
    }

    // Clase para mapear la respuesta de la API
    public class AuthResponse
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public UserProfile? UserProfile { get; set; }
    }
}
