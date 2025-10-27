namespace InclusingLenguage._03_Views;

using InclusingLenguage._01_Models;
using InclusingLenguage._04_Services;
using InclusingLenguage._05_Helpers;

public partial class RegisterPage : ContentPage
{
    private readonly IAuthenticationService _authService;

    public RegisterPage(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        try
        {
            // Validar que todos los campos estén completos
            if (string.IsNullOrWhiteSpace(FirstNameEntry.Text))
            {
                await DisplayAlert("Error", "Por favor ingresa tu nombre", "OK");
                FirstNameEntry.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailEntry.Text))
            {
                await DisplayAlert("Error", "Por favor ingresa tu correo electrónico", "OK");
                EmailEntry.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Error", "Por favor ingresa una contraseña", "OK");
                PasswordEntry.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
            {
                await DisplayAlert("Error", "Por favor confirma tu contraseña", "OK");
                ConfirmPasswordEntry.Focus();
                return;
            }

            // Validar contraseñas coinciden
            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
                ConfirmPasswordEntry.Text = "";
                ConfirmPasswordEntry.Focus();
                return;
            }

            // Validar longitud de contraseña
            if (PasswordEntry.Text.Length < 6)
            {
                await DisplayAlert("Error", "La contraseña debe tener al menos 6 caracteres", "OK");
                return;
            }

            // Validar términos y condiciones
            if (!TermsCheckBox.IsChecked)
            {
                await DisplayAlert("Error",
                    "Debes aceptar los Términos y Condiciones para continuar",
                    "OK");
                return;
            }

            // Validar formato de email
            if (!IsValidEmail(EmailEntry.Text))
            {
                await DisplayAlert("Error",
                    "Por favor ingresa un correo electrónico válido",
                    "OK");
                EmailEntry.Focus();
                return;
            }

            // Mostrar loading
            ShowLoading(true, "Creando tu cuenta...");
            RegisterButton.IsEnabled = false;

            // Crear request de registro
            var registerRequest = new RegisterRequest
            {
                FirstName = FirstNameEntry.Text.Trim(),
                LastName = LastNameEntry.Text?.Trim() ?? "",
                Email = EmailEntry.Text.Trim(),
                Password = PasswordEntry.Text,
                ConfirmPassword = ConfirmPasswordEntry.Text
            };

            // Intentar registro
            var result = await _authService.RegisterAsync(registerRequest);

            ShowLoading(false);

            if (result.IsSuccess)
            {
                // Guardar datos del usuario
                await SecureStorage.SetAsync("user_token", result.Token);
                await SecureStorage.SetAsync("user_email", result.UserProfile.Email);
                await SecureStorage.SetAsync("is_guest", "false");
                await SecureStorage.SetAsync("is_new_user", "true");

                // Mostrar mensaje de bienvenida
                await DisplayAlert("¡Bienvenido a SignLearn! 🎉",
                    $"Hola {result.UserProfile.FirstName}, tu cuenta ha sido creada exitosamente.\n\n" +
                    "¡Estás listo para comenzar tu viaje en el lenguaje de señas!",
                    "¡Empecemos!");

                // Limpiar la pila de navegación y ir a HomePage
                Application.Current.MainPage = new NavigationPage(ServiceHelper.GetService<HomePage>())
                {
                    BarBackgroundColor = Color.FromArgb("#00131F"),
                    BarTextColor = Colors.White
                };
            }
            else
            {
                // Manejar errores específicos
                if (result.ErrorCode == "USER_EXISTS")
                {
                    bool goToLogin = await DisplayAlert(
                        "Cuenta existente",
                        "Ya existe una cuenta con este correo electrónico. ¿Deseas iniciar sesión?",
                        "Sí, iniciar sesión",
                        "No");

                    if (goToLogin)
                    {
                        await Navigation.PopAsync();
                    }
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
            RegisterButton.IsEnabled = true;
        }
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        // Volver a la página de login
        await Navigation.PopAsync();
    }

    private async void OnTermsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Términos y Condiciones",
            "TÉRMINOS Y CONDICIONES DE USO - SignLearn\n\n" +
            "1. Aceptación de Términos\n" +
            "Al usar SignLearn, aceptas estos términos.\n\n" +
            "2. Uso del Servicio\n" +
            "SignLearn es una plataforma educativa para aprender lenguaje de señas.\n\n" +
            "3. Privacidad\n" +
            "Respetamos tu privacidad y protegemos tus datos personales.\n\n" +
            "4. Contenido\n" +
            "Todo el contenido es propiedad de SignLearn y está protegido por derechos de autor.\n\n" +
            "Para ver los términos completos, visita: www.signlearn.com/terminos",
            "Cerrar");
    }

    private void ShowLoading(bool show, string message = "Cargando...")
    {
        LoadingLayout.IsVisible = show;
        LoadingIndicator.IsRunning = show;
        LoadingLabel.Text = message;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}