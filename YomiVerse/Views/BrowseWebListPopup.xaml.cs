using CommunityToolkit.Maui.Views;
using System.Collections.Generic;
using System.Windows.Input;
using YomiVerse.Services;

namespace YomiVerse.Views
{
    public partial class BrowseWebListPopup : Popup
    {
        public List<BrowseWebItems> MangaList { get; set; }

        public ICommand ItemTappedCommand { get; }

        public BrowseWebListPopup(List<BrowseWebItems> items)
        {
            InitializeComponent();
            MangaList = items;
            ItemTappedCommand = new Command<BrowseWebItems>(async (item) => await OnItemTapped(item));
            BindingContext = this;
        }

        private async Task OnItemTapped(BrowseWebItems item)
        {
            var popup = new ComicInfoPopup(item);
            Application.Current.MainPage.ShowPopup(popup);
        }
    }

}
