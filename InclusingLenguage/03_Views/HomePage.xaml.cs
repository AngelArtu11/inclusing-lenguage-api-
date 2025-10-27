namespace InclusingLenguage._03_Views;

using InclusingLenguage._01_Models;
using InclusingLenguage._04_Services;
using InclusingLenguage._05_Helpers;

public partial class HomePage : ContentPage
{
    private readonly IAuthenticationService _authService;
    private readonly ILessonService _lessonService;
    private UserProfile _currentUser;

    public HomePage(IAuthenticationService authService, ILessonService lessonService)
    {
        InitializeComponent();
        _authService = authService;
        _lessonService = lessonService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserData();
    }

    private async Task LoadUserData()
    {
        try
        {
            _currentUser = await _authService.GetCurrentUserAsync();

            if (_currentUser != null)
            {
                // Actualizar UI con datos del usuario
                if (_currentUser.IsGuest)
                {
                    WelcomeLabel.Text = "¡Hola, Invitado!";
                    UserInitialLabel.Text = "??";
                    UserLevelLabel.Text = "Modo Invitado";
                    ExperienceLabel.Text = "0";
                    StreakLabel.Text = "0";
                    DailyGoalLabel.Text = "0/5";
                    DailyProgressBar.Progress = 0;
                }
                else
                {
                    // Usuario registrado
                    WelcomeLabel.Text = $"¡Hola, {_currentUser.FirstName}!";

                    // Obtener inicial del nombre
                    UserInitialLabel.Text = _currentUser.FirstName.Substring(0, 1).ToUpper();

                    UserLevelLabel.Text = $"Nivel {_currentUser.Level}";
                    ExperienceLabel.Text = _currentUser.Experience.ToString();
                    StreakLabel.Text = _currentUser.Streak.ToString();

                    // Actualizar progreso diario
                    DailyGoalLabel.Text = $"{_currentUser.TodayProgress}/{_currentUser.DailyGoal}";
                    DailyProgressBar.Progress = (double)_currentUser.TodayProgress / _currentUser.DailyGoal;
                }

                // Verificar si es usuario nuevo para mostrar bienvenida especial
                var isNewUser = await SecureStorage.GetAsync("is_new_user");
                if (isNewUser == "true")
                {
                    await SecureStorage.SetAsync("is_new_user", "false");
                    await ShowWelcomeTutorial();
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo cargar la información del usuario", "OK");
        }
    }

    private async Task ShowWelcomeTutorial()
    {
        await DisplayAlert("¡Bienvenido a SignLearn! ??",
            "Estamos emocionados de tenerte aquí.\n\n" +
            "?? Comienza con el alfabeto\n" +
            "?? Completa lecciones para ganar XP\n" +
            "?? Mantén tu racha diaria\n" +
            "? Sube de nivel y desbloquea contenido\n\n" +
            "¡Empecemos tu viaje en el lenguaje de señas!",
            "¡Vamos!");
    }

    private async void OnContinueLessonTapped(object sender, EventArgs e)
    {
        try
        {
            var nextLesson = await _lessonService.GetNextIncompleteLessonAsync("Alphabet");

            if (nextLesson != null)
            {
                var factory = ServiceHelper.GetService<LessonPageFactory>();
                await Navigation.PushAsync(factory.Create(nextLesson.Id));
            }
            else
            {
                await DisplayAlert("¡Felicidades!",
                    "Has completado todas las lecciones del alfabeto.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
    }

    private async void OnPracticeTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Modo Práctica",
            "¡Practica lo que has aprendido!\n\n" +
            "Esta función estará disponible pronto. Podrás repasar:\n" +
            "• Alfabeto completo\n" +
            "• Números\n" +
            "• Palabras básicas\n" +
            "• Frases comunes",
            "Entendido");
    }

    private async void OnAlphabetLessonTapped(object sender, EventArgs e)
    {
        try
        {
            var userEmail = await SecureStorage.GetAsync("user_email");
            if (string.IsNullOrEmpty(userEmail))
            {
                await DisplayAlert("Error", "No se pudo obtener el email del usuario", "OK");
                return;
            }

            var completedCount = await _lessonService.GetCompletedLessonsCountAsync("Alphabet", userEmail);
            var totalCount = 27;
            var percentage = (completedCount * 100) / totalCount;

            bool startLesson = await DisplayAlert("Alfabeto en Señas",
                $"📚 {totalCount} lecciones\n" +
                $"⭐ Nivel: Básico\n" +
                $"⏱️ 5 min por lección\n\n" +
                $"Progreso: {completedCount} de {totalCount} ({percentage}%)",
                "Comenzar",
                "Cancelar");

            if (startLesson)
            {
                var nextLesson = await _lessonService.GetNextIncompleteLessonAsync("Alphabet");
                if (nextLesson != null)
                {
                    var factory = ServiceHelper.GetService<LessonPageFactory>();
                    await Navigation.PushAsync(factory.Create(nextLesson.Id));
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
    }

    private async void OnLessonsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Lecciones",
            "Vista de todas las lecciones disponibles.\n\n" +
            "Próximamente podrás ver:\n" +
            "• Todas las categorías\n" +
            "• Tu progreso detallado\n" +
            "• Lecciones recomendadas\n" +
            "• Certificados obtenidos",
            "OK");
    }

    private async void OnProgressTapped(object sender, EventArgs e)
    {
        if (_currentUser != null && !_currentUser.IsGuest)
        {
            string progressMessage = $"?? TU PROGRESO\n\n" +
                $"Nivel: {_currentUser.Level}\n" +
                $"Experiencia: {_currentUser.Experience} XP\n" +
                $"Racha: {_currentUser.Streak} días ??\n" +
                $"Lecciones completadas: {_currentUser.CompletedLessons.Count}\n" +
                $"Meta diaria: {_currentUser.TodayProgress}/{_currentUser.DailyGoal}\n\n" +
                $"¡Sigue así! ??";

            await DisplayAlert("Tu Progreso", progressMessage, "OK");
        }
        else
        {
            bool shouldRegister = await DisplayAlert("Modo Invitado",
                "Como invitado tu progreso no se guarda.\n\n" +
                "¿Deseas crear una cuenta para guardar tu progreso?",
                "Sí, crear cuenta",
                "Ahora no");

            if (shouldRegister)
            {
                var registerPage = ServiceHelper.GetService<RegisterPage>();
                await Navigation.PushAsync(registerPage);
            }
        }
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        // Simplemente navegar a ProfilePage
        var profilePage = ServiceHelper.GetService<ProfilePage>();
        await Navigation.PushAsync(profilePage);
    }
}