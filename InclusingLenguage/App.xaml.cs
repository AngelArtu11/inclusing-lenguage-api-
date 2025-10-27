namespace InclusingLenguage
{
    public partial class App : Application
    {
        public App(IServiceProvider services)
        {
            InitializeComponent();

            var loginPage = services.GetRequiredService<_03_Views.LoginPage>();
            MainPage = new NavigationPage(loginPage)
            {
                BarBackgroundColor = Color.FromArgb("#00131F"),
                BarTextColor = Colors.White
            };
        }
    }
}
