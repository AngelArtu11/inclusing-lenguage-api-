namespace InclusingLenguage._03_Views;

using InclusingLenguage._01_Models;
using InclusingLenguage._04_Services;
using InclusingLenguage._05_Helpers;


public partial class LoginPage : ContentPage
{
    private readonly IAuthenticationService _authService;

    public LoginPage(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Verificar si el usuario ya está logueado
        if (await _authService.IsUserLoggedInAsync())
        {
            await Navigation.PushAsync(ServiceHelper.GetService<HomePage>());
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            // Validar campos vacíos
            if (string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Error", "Por favor completa todos los campos", "OK");
                return;
            }

            // Mostrar loading
            ShowLoading(true, "Iniciando sesión...");
            LoginButton.IsEnabled = false;

            // Crear request de login
            var loginRequest = new LoginRequest
            {
                Email = EmailEntry.Text.Trim(),
                Password = PasswordEntry.Text,
                RememberMe = RememberMeCheckBox.IsChecked
            };

            // Intentar login
            var result = await _authService.LoginAsync(loginRequest);

            ShowLoading(false);

            if (result.IsSuccess)
            {
                // Guardar datos del usuario
                await SecureStorage.SetAsync("user_token", result.Token);
                await SecureStorage.SetAsync("user_email", result.UserProfile.Email);
                await SecureStorage.SetAsync("is_guest", "false");

                // Mostrar mensaje de bienvenida
                await DisplayAlert("¡Bienvenido!",
                    $"Hola {result.UserProfile.FirstName}, ¡es genial verte de nuevo!",
                    "Comenzar");

                // Navegar a la página principal
                await Navigation.PushAsync(ServiceHelper.GetService<HomePage>());
            }
            else
            {
                // Manejar errores específicos
                if (result.ErrorCode == "USER_NOT_FOUND")
                {
                    bool shouldRegister = await DisplayAlert(
                        "Usuario no encontrado",
                        "No existe una cuenta con este correo. ¿Deseas crear una cuenta nueva?",
                        "Sí, crear cuenta",
                        "No");

                    if (shouldRegister)
                    {
                        await Navigation.PushAsync(ServiceHelper.GetService<RegisterPage>());
                    }
                }
                else if (result.ErrorCode == "INVALID_PASSWORD")
                {
                    await DisplayAlert("Error",
                        "Contraseña incorrecta. Por favor verifica e intenta de nuevo.",
                        "OK");
                    PasswordEntry.Text = "";
                    PasswordEntry.Focus();
                }
                else
                {
                    await DisplayAlert("Error", result.ErrorMessage, "OK");
                }
            }
        }
        catch (Exception ex)
        {
            ShowLoading(false);
            await DisplayAlert("Error",
                "Ocurrió un error inesperado. Por favor intenta de nuevo.",
                "OK");
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        // Navegar a la página de registro
        await Navigation.PushAsync(ServiceHelper.GetService<RegisterPage>());
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Recuperar contraseña",
            "Esta función estará disponible próximamente. Por favor contacta a soporte.",
            "OK");
    }

    private async void OnGuestClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Modo Invitado",
            "Como invitado podrás explorar la app pero tu progreso no se guardará. ¿Deseas continuar?",
            "Sí, continuar",
            "Cancelar");

        if (confirm)
        {
            // Marcar como usuario invitado
            await SecureStorage.SetAsync("is_guest", "true");
            await SecureStorage.SetAsync("user_email", "guest@signlearn.com");
            await SecureStorage.SetAsync("user_token", "guest_token");

            // Navegar a la página principal
            await Navigation.PushAsync(ServiceHelper.GetService<HomePage>());
        }
    }

    private async void OnQuickTourTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Tour Rápido",
            "¡Bienvenido a SignLearn! 🤟\n\n" +
            "• Aprende el alfabeto en señas\n" +
            "• Practica con ejercicios interactivos\n" +
            "• Gana experiencia y sube de nivel\n" +
            "• Mantén tu racha diaria\n\n" +
            "¡Comienza tu viaje hoy mismo!",
            "Entendido");
    }

    private async void OnHelpTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Ayuda",
            "¿Necesitas ayuda?\n\n" +
            "📧 Email: soporte@signlearn.com\n" +
            "💬 Chat en vivo disponible 24/7\n" +
            "📖 Visita nuestra sección de Preguntas Frecuentes",
            "OK");
    }

    private void ShowLoading(bool show, string message = "Cargando...")
    {
        LoadingLayout.IsVisible = show;
        LoadingIndicator.IsRunning = show;
        LoadingLabel.Text = message;
    }
}