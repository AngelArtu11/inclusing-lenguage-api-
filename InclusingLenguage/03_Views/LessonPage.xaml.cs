namespace InclusingLenguage._03_Views;

using InclusingLenguage._01_Models;
using InclusingLenguage._04_Services;
using InclusingLenguage._05_Helpers;

public partial class LessonPage : ContentPage
{
    private readonly ILessonService _lessonService;
    private readonly IAuthenticationService _authService;
    private Lesson _currentLesson;
    private int _currentExerciseIndex = 0;
    private int _score = 0;
    private int _totalPoints = 0;
    private string _selectedAnswer = "";
    private bool _answerVerified = false;

    public LessonPage(ILessonService lessonService, IAuthenticationService authService, int lessonId)
    {
        InitializeComponent();
        _lessonService = lessonService;
        _authService = authService;

        // Resetear estado
        _currentExerciseIndex = 0;
        _score = 0;
        _selectedAnswer = "";
        _answerVerified = false;

        LoadLesson(lessonId);
    }

    private async void LoadLesson(int lessonId)
    {
        try
        {
            _currentLesson = await _lessonService.GetLessonByIdAsync(lessonId);

            if (_currentLesson != null)
            {
                LessonTitleLabel.Text = _currentLesson.Title;
                LessonXPLabel.Text = _currentLesson.ExperiencePoints.ToString();

                _totalPoints = _currentLesson.Exercises.Sum(e => e.Points);
                ShowCurrentExercise();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo cargar la lección", "OK");
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            await Navigation.PopAsync();
        }
    }

    private void ShowCurrentExercise()
    {
        if (_currentExerciseIndex >= _currentLesson.Exercises.Count)
        {
            CompleteLessonAsync();
            return;
        }

        var exercise = _currentLesson.Exercises[_currentExerciseIndex];

        // Actualizar progreso
        var progress = (double)(_currentExerciseIndex) / _currentLesson.Exercises.Count;
        LessonProgressBar.Progress = progress;
        ProgressLabel.Text = $"{_currentExerciseIndex + 1}/{_currentLesson.Exercises.Count}";

        // Resetear estado
        FeedbackFrame.IsVisible = false;
        HintFrame.IsVisible = false;
        HintButton.IsEnabled = true;
        HintButton.Text = "💡 Pista";
        _answerVerified = false;
        _selectedAnswer = "";

        // Mostrar el tipo correcto
        PracticeExercise.IsVisible = false;
        MultipleChoiceExercise.IsVisible = false;

        switch (exercise.Type)
        {
            case ExerciseType.Practice:
                ShowPracticeExercise(exercise);
                break;
            case ExerciseType.MultipleChoice:
            case ExerciseType.SignRecognition:
                ShowMultipleChoiceExercise(exercise);
                break;
        }
    }

    private void ShowPracticeExercise(Exercise exercise)
    {
        PracticeExercise.IsVisible = true;
        ExerciseQuestionLabel.Text = exercise.Question;
        DescriptionLabel.Text = exercise.HintText;
        SignEmojiLabel.Text = _currentLesson.ImageUrl;  
        SignImagePlaceholder.Text = $"Letra {_currentLesson.Letter}";  

        NextButton.Text = "Continuar";
       // NextButton.IsEnabled = true;
    }

    private void ShowMultipleChoiceExercise(Exercise exercise)
    {
        MultipleChoiceExercise.IsVisible = true;
        MCQuestionLabel.Text = exercise.Question;

        // Mostrar emoji o letra según el ejercicio
        var exerciseFrame = MultipleChoiceExercise.Children[0] as Frame;
        if (exerciseFrame != null)
        {
            var stack = exerciseFrame.Content as StackLayout;
            if (stack != null && stack.Children.Count > 1)
            {
                var imageFrame = stack.Children[1] as Frame;
                if (imageFrame != null)
                {
                    var emojiLabel = imageFrame.Content as Label;
                    if (emojiLabel != null)
                    {
                        // ✅ Solo en el ejercicio 3 mostrar la letra, en el resto mostrar emoji
                        if (exercise.Id == 3)
                        {
                            emojiLabel.Text = _currentLesson.Letter; // Solo la letra "A"
                            emojiLabel.FontSize = 150;
                        }
                        else
                        {
                            emojiLabel.Text = _currentLesson.ImageUrl; // El emoji 🤘
                            emojiLabel.FontSize = 100;
                        }
                        emojiLabel.TextColor = Color.FromArgb("#13B7FF");
                    }
                }
            }
        }

        // Configurar opciones
        if (exercise.Options.Count >= 4)
        {
            UpdateOptionLabels(exercise.Options);
        }

        ResetOptionColors();
        EnableOptions(true);
        HintLabel.Text = exercise.HintText;

        NextButton.Text = "Verificar";
        NextButton.IsEnabled = false;
    }

    private void UpdateOptionLabels(List<string> options)
    {
        var labels = new[] { "A)", "B)", "C)", "D)" };
        var frames = new[] { Option1, Option2, Option3, Option4 };

        for (int i = 0; i < Math.Min(options.Count, 4); i++)
        {
            if (frames[i].Content is Label label)
            {
                label.Text = $"{labels[i]} {options[i]}";
            }
        }
    }

    private async void OnOptionTapped(object sender, EventArgs e)
    {
        if (_answerVerified) return;

        if (sender is Frame frame && frame.GestureRecognizers[0] is TapGestureRecognizer tap)
        {
            var exercise = _currentLesson.Exercises[_currentExerciseIndex];
            var optionIndex = frame == Option1 ? 0 : frame == Option2 ? 1 : frame == Option3 ? 2 : 3;

            if (optionIndex < exercise.Options.Count)
            {
                _selectedAnswer = exercise.Options[optionIndex];

                ResetOptionColors();
                frame.BackgroundColor = Color.FromArgb("#13B7FF");
                frame.BorderColor = Color.FromArgb("#13B7FF");

                // Habilitar el botón cuando selecciona una opción
                NextButton.IsEnabled = true;
                NextButton.Text = "Verificar";
            }
        }
    }

    private void ResetOptionColors()
    {
        var frames = new[] { Option1, Option2, Option3, Option4 };
        foreach (var frame in frames)
        {
            frame.BackgroundColor = Color.FromArgb("#00577D");
            frame.BorderColor = Color.FromArgb("#0073A3");
        }
    }

    private void EnableOptions(bool enable)
    {
        Option1.IsEnabled = enable;
        Option2.IsEnabled = enable;
        Option3.IsEnabled = enable;
        Option4.IsEnabled = enable;
    }

    private void OnHintClicked(object sender, EventArgs e)
    {
        HintFrame.IsVisible = !HintFrame.IsVisible;
        HintButton.Text = HintFrame.IsVisible ? "🙈 Ocultar" : "💡 Pista";
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        // Deshabilitar el botón para evitar clics múltiples
        NextButton.IsEnabled = false;
        HintButton.IsEnabled = false;

        try
        {
            var currentExercise = _currentLesson.Exercises[_currentExerciseIndex];

            // Si es práctica, solo avanzar
            if (currentExercise.Type == ExerciseType.Practice)
            {
                _score += currentExercise.Points;
                _currentExerciseIndex++;
                ShowCurrentExercise();
               // NextButton.IsEnabled = true;
                HintButton.IsEnabled = true;
                return;
            }

            // Si no ha verificado la respuesta
            if (!_answerVerified && NextButton.Text == "Verificar")
            {
                if (string.IsNullOrEmpty(_selectedAnswer))
                {
                    await DisplayAlert("Atención", "Por favor selecciona una respuesta antes de verificar", "OK");
                    //NextButton.IsEnabled = true;
                    HintButton.IsEnabled = true;
                    return;
                }

                // Verificar si la respuesta es correcta
                bool isCorrect = _selectedAnswer == currentExercise.CorrectAnswer;

                MarkCorrectAnswer(currentExercise);
                ShowFeedback(isCorrect, currentExercise.Points);

                if (isCorrect)
                {
                    _score += currentExercise.Points;
                }

                _answerVerified = true;
                EnableOptions(false);

                NextButton.Text = _currentExerciseIndex < _currentLesson.Exercises.Count - 1
                    ? "Siguiente ➡️"
                    : "Finalizar 🎉";
                NextButton.IsEnabled = true;
                HintButton.IsEnabled = true;
            }
            else if (_answerVerified)
            {
                // Avanzar al siguiente ejercicio
                if (_currentExerciseIndex < _currentLesson.Exercises.Count - 1)
                {
                    _currentExerciseIndex++;
                    ShowCurrentExercise();
                    NextButton.IsEnabled = true;
                    HintButton.IsEnabled = true;
                }
                else
                {
                    // Es la última lección, completar
                    await CompleteLessonAsync();
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            NextButton.IsEnabled = true;
            HintButton.IsEnabled = true;
        }
    }

    private void MarkCorrectAnswer(Exercise exercise)
    {
        var frames = new[] { Option1, Option2, Option3, Option4 };

        for (int i = 0; i < Math.Min(exercise.Options.Count, frames.Length); i++)
        {
            var option = exercise.Options[i];
            var frame = frames[i];

            if (option == exercise.CorrectAnswer)
            {
                // Opción correcta en verde
                frame.BackgroundColor = Color.FromArgb("#4CAF50");
                frame.BorderColor = Color.FromArgb("#4CAF50");
            }
            else if (option == _selectedAnswer && _selectedAnswer != exercise.CorrectAnswer)
            {
                // Opción incorrecta seleccionada en rojo
                frame.BackgroundColor = Color.FromArgb("#FF6B6B");
                frame.BorderColor = Color.FromArgb("#FF6B6B");
            }
        }
    }

    private void ShowFeedback(bool isCorrect, int points)
    {
        FeedbackFrame.IsVisible = true;

        if (isCorrect)
        {
            FeedbackFrame.BackgroundColor = Color.FromArgb("#4CAF50");
            FeedbackIcon.Text = "✅";
            FeedbackTitle.Text = "¡Correcto!";
            FeedbackTitle.TextColor = Colors.White;
            FeedbackMessage.Text = "¡Excelente! Has acertado la respuesta.";
            FeedbackMessage.TextColor = Colors.White;
            FeedbackXP.Text = $"+{points} XP";
            FeedbackXP.TextColor = Color.FromArgb("#FFD700");
        }
        else
        {
            var correctAnswer = _currentLesson.Exercises[_currentExerciseIndex].CorrectAnswer;
            FeedbackFrame.BackgroundColor = Color.FromArgb("#FF6B6B");
            FeedbackIcon.Text = "❌";
            FeedbackTitle.Text = "Incorrecto";
            FeedbackTitle.TextColor = Colors.White;
            FeedbackMessage.Text = $"La respuesta correcta es: {correctAnswer}";
            FeedbackMessage.TextColor = Colors.White;
            FeedbackXP.Text = "+0 XP";
            FeedbackXP.TextColor = Colors.White;
        }
    }

    private async Task CompleteLessonAsync()
    {
        try
        {
            // Obtener email del usuario
            var userEmail = await SecureStorage.GetAsync("user_email");
            if (string.IsNullOrEmpty(userEmail))
            {
                await DisplayAlert("Error", "No se pudo obtener el email del usuario", "OK");
                return;
            }

            double percentage = (_score * 100.0) / _totalPoints;

            // Completar lección en MongoDB
            await _lessonService.CompleteLessonAsync(_currentLesson.Id, (int)percentage, userEmail);

            // Actualizar experiencia del usuario
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser != null && !currentUser.IsGuest)
            {
                currentUser.Experience += _score;
                currentUser.TodayProgress++;

                int newLevel = (currentUser.Experience / 100) + 1;
                currentUser.Level = newLevel;
            }

            string emoji = percentage >= 90 ? "🎉" : percentage >= 70 ? "👏" : "📚";
            string title = percentage >= 90 ? "¡PERFECTO!" : percentage >= 70 ? "¡Bien hecho!" : "Lección completada";

            string message = $"{emoji} {title}\n\n" +
                           $"Puntuación: {percentage:F0}%\n" +
                           $"⚡ +{_score} XP ganados\n\n" +
                           (percentage >= 90 ? "¡Dominas esta letra!" :
                            percentage >= 70 ? "¡Muy buen trabajo!" :
                            "Puedes repetir para mejorar");

            bool continueLearning = await DisplayAlert(
                "Lección Completada",
                message,
                "Siguiente lección",
                "Volver al inicio");

            if (continueLearning)
            {
                var nextLesson = await _lessonService.GetNextIncompleteLessonAsync(_currentLesson.Category);
                if (nextLesson != null && nextLesson.Id != _currentLesson.Id)
                {
                    Navigation.RemovePage(this);
                    await Navigation.PushAsync(ServiceHelper.GetService<LessonPageFactory>().Create(nextLesson.Id));
                }
                else
                {
                    await DisplayAlert("¡Felicidades! 🎊",
                        "Has completado todas las lecciones disponibles.",
                        "Genial");
                    await Navigation.PopAsync();
                }
            }
            else
            {
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error completando lección: {ex.Message}", "OK");
        }
    }

    private async void OnPlayVideoTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Video Tutorial",
            $"Video demostrativo de la letra {_currentLesson.Letter}\n\n" +
            "Esta función estará disponible próximamente con videos reales.",
            "Entendido");
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Salir",
            "¿Deseas salir? Tu progreso en esta lección se perderá.",
            "Sí",
            "No");

        if (confirm)
        {
            await Navigation.PopAsync();
        }
    }
}