using YomiVerse.ViewModels;

namespace YomiVerse.Views
{
    public partial class BrowseView : ContentView
    {
        public BrowseView()
        {
            InitializeComponent();

            // Load saved sources once the view appears
            this.Loaded += async (s, e) =>
            {
                if (BindingContext is BrowseVewModel vm)
                {
                    await vm.RefreshSourcesOnLoad();
                }
            };
        }
    }
}
