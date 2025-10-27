namespace InclusingLenguage._05_Helpers
{
    public static class ServiceHelper
    {
        public static TService GetService<TService>()
            where TService : class
        {
#if WINDOWS
            return MauiWinUIApplication.Current.Services.GetService<TService>()!;
#elif ANDROID
            return IPlatformApplication.Current!.Services.GetService<TService>()!;
#elif IOS || MACCATALYST
            return MauiUIApplicationDelegate.Current.Services.GetService<TService>()!;
#else
            throw new PlatformNotSupportedException();
#endif
        }
    }
}
