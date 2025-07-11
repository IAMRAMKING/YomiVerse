using YomiVerse.Views;

namespace YomiVerse
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new LandingPageView();
        }
    }
}
