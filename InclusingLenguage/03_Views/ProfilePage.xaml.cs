namespace InclusingLenguage._03_Views;

using InclusingLenguage._01_Models;
using InclusingLenguage._04_Services;
using InclusingLenguage._05_Helpers;

public partial class ProfilePage : ContentPage
{
    private readonly IProfileService _profileService;
    private readonly IAuthenticationService _authService;
    private UserProfileExtended _userProfileExtended;
    private string _currentUserEmail;

    public ProfilePage(IProfileService profileService, IAuthenticationService authService)
    {
        InitializeComponent();
        _profileService = profileService;
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserProfile();
    }

    private async Task LoadUserProfile()
    {
        try
        {
            // Obtener email del usuario actual
            _currentUserEmail = await SecureStorage.GetAsync("user_email");
            var isGuest = await SecureStorage.GetAsync("is_guest");

            if (isGuest == "true")
            {
                ShowGuestMode();
                return;
            }

            if (string.IsNullOrEmpty(_currentUserEmail))
            {
                await DisplayAlert("Error", "No se pudo obtener la información del usuario", "OK");
                return;
            }

            // Cargar perfil extendido del usuario
            _userProfileExtended = await _profileService.GetUserProfileExtendedAsync(_currentUserEmail);

            if (_userProfileExtended != null)
            {
                PopulateProfileUI();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo cargar el perfil del usuario", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }

    private void ShowGuestMode()
    {
        UserNameLabel.Text = "Usuario Invitado";
        UserEmailLabel.Text = "guest@signlearn.com";
        MemberSinceLabel.Text = "Modo invitado - Tu progreso no se guarda";
        UserAvatarLabel.Text = "👤";
    }

    private void PopulateProfileUI()
    {
        var basicInfo = _userProfileExtended.BasicInfo;
        var stats = _userProfileExtended.Statistics;
        var badges = _userProfileExtended.Badges;

        // Información básica
        UserNameLabel.Text = basicInfo.Name;
        UserEmailLabel.Text = basicInfo.Email;
        UserAvatarLabel.Text = basicInfo.FirstName.Substring(0, 1).ToUpper();

        // Calcular tiempo como miembro
        var daysAsMember = (DateTime.Now - stats.MemberSince).Days;
        MemberSinceLabel.Text = daysAsMember >= 30
            ? $"Miembro hace {daysAsMember / 30} meses"
            : $"Miembro hace {daysAsMember} días";

        // Estadísticas principales
        UserLevelDisplay.Text = basicInfo.Level.ToString();
        StreakDisplay.Text = $"{stats.CurrentStreak} días";
        TotalXPDisplay.Text = basicInfo.Experience.ToString();

        // Estadísticas detalladas
        LessonsCompletedLabel.Text = $"{stats.TotalLessonsCompleted} completadas";
        TotalMinutesLabel.Text = $"{stats.TotalMinutesPracticed} minutos";
        AverageScoreLabel.Text = $"{stats.AverageScore}%";
        SessionsLabel.Text = $"{stats.TotalPracticeSessions} sesiones";

        // Cargar badges
        LoadBadges(badges);

        // Cargar progreso por categoría
        LoadCategoryProgress(stats);
    }

    private void LoadBadges(List<Badge> badges)
    {
        BadgesCollectionView.ItemsSource = badges;
    }

    private void LoadCategoryProgress(UserStats stats)
    {
        // Alfabeto
        if (stats.CategoryProgress.ContainsKey("Alphabet"))
        {
            int alphabetCompleted = stats.CategoryProgress["Alphabet"];
            AlphabetProgressLabel.Text = $"{alphabetCompleted}/27";
            AlphabetProgressBar.Progress = alphabetCompleted / 27.0;
        }

        // Números
        if (stats.CategoryProgress.ContainsKey("Numbers"))
        {
            int numbersCompleted = stats.CategoryProgress["Numbers"];
            // Mostrar en UI (si existe en el XAML)
        }

        // Palabras
        if (stats.CategoryProgress.ContainsKey("Words"))
        {
            int wordsCompleted = stats.CategoryProgress["Words"];
            // Mostrar en UI (si existe en el XAML)
        }
    }

    private async void OnEditProfileTapped(object sender, EventArgs e)
    {
        if (_userProfileExtended == null) return;

        // Crear diálogo para editar
        var newName = await DisplayPromptAsync(
            "Editar Nombre",
            "Ingresa tu nuevo nombre:",
            initialValue: _userProfileExtended.BasicInfo.Name,
            maxLength: 50);

        if (!string.IsNullOrEmpty(newName) && newName != _userProfileExtended.BasicInfo.Name)
        {
            _userProfileExtended.BasicInfo.Name = newName;
            var parts = newName.Split(' ');
            _userProfileExtended.BasicInfo.FirstName = parts[0];
            _userProfileExtended.BasicInfo.LastName = parts.Length > 1 ? parts[1] : "";

            // Actualizar en el servicio
            bool updated = await _profileService.UpdateUserProfileAsync(_userProfileExtended.BasicInfo);

            if (updated)
            {
                UserNameLabel.Text = newName;
                await DisplayAlert("Éxito", "Tu perfil ha sido actualizado", "OK");
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar el perfil", "OK");
            }
        }
    }

    private async void OnSettingsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Configuración",
            "Próximamente podrás configurar:\n\n" +
            "⏰ Recordatorios diarios\n" +
            "🔊 Sonidos y efectos\n" +
            "🌙 Modo oscuro/claro\n" +
            "🗣️ Idioma de la app\n" +
            "🎯 Meta diaria\n" +
            "📱 Privacidad",
            "OK");
    }

    private async void OnAchievementsTapped(object sender, EventArgs e)
    {
        if (_userProfileExtended == null) return;

        var badges = _userProfileExtended.Badges;
        var totalAchievements = 15; // Total posible de logros

        string achievementMessage = $"🏆 TUS LOGROS\n\n" +
            $"Desbloqueados: {badges.Count}/{totalAchievements}\n\n";

        foreach (var badge in badges.Take(5))
        {
            achievementMessage += $"{badge.Icon} {badge.Title}\n";
            achievementMessage += $"   {badge.Reason}\n\n";
        }

        if (badges.Count > 5)
        {
            achievementMessage += $"... y {badges.Count - 5} más logros";
        }

        await DisplayAlert("Logros", achievementMessage, "OK");
    }

    private async void OnStatisticsTapped(object sender, EventArgs e)
    {
        if (_userProfileExtended == null) return;

        var stats = _userProfileExtended.Statistics;

        string statisticsMessage = $"📊 ESTADÍSTICAS DETALLADAS\n\n" +
            $"Lecciones completadas: {stats.TotalLessonsCompleted}\n" +
            $"XP total: {stats.TotalExperienceGained}\n" +
            $"Racha actual: {stats.CurrentStreak} días\n" +
            $"Racha más larga: {stats.LongestStreak} días\n" +
            $"Promedio de puntuación: {stats.AverageScore}%\n" +
            $"Sesiones de práctica: {stats.TotalPracticeSessions}\n" +
            $"Minutos practicados: {stats.TotalMinutesPracticed}\n" +
            $"Último activo: {stats.LastActiveDate.ToString("dd/MM/yyyy HH:mm")}\n\n" +
            $"Miembro desde: {stats.MemberSince.ToString("dd/MM/yyyy")}";

        await DisplayAlert("Estadísticas Completas", statisticsMessage, "OK");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Cerrar Sesión",
            "¿Estás seguro que deseas cerrar sesión?",
            "Sí, cerrar sesión",
            "Cancelar");

        if (confirm)
        {
            // Cerrar sesión
            await _authService.LogoutAsync();

            // Volver a login
            Application.Current.MainPage = new NavigationPage(ServiceHelper.GetService<LoginPage>())
            {
                BarBackgroundColor = Color.FromArgb("#00131F"),
                BarTextColor = Colors.White
            };
        }
    }
}