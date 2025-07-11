using Microsoft.Maui.Controls;
using YomiVerse.Views;

namespace YomiVerse
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new LandingPageView());
        }
    }
}
