using InclusingLenguage._03_Views;
using InclusingLenguage._04_Services;

namespace InclusingLenguage._05_Helpers
{
    public class LessonPageFactory
    {
        private readonly ILessonService _lessonService;
        private readonly IAuthenticationService _authService;

        public LessonPageFactory(ILessonService lessonService, IAuthenticationService authService)
        {
            _lessonService = lessonService;
            _authService = authService;
        }

        public LessonPage Create(int lessonId)
        {
            return new LessonPage(_lessonService, _authService, lessonId);
        }
    }
}
