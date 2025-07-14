using CommunityToolkit.Maui.Views;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows.Input;
using YomiVerse.Services;
using YomiVerse.ViewModels;

namespace YomiVerse.Views;

public partial class BrowseWebListPopup : Popup
{
    private readonly BrowseVewModel _viewModel;

    public ObservableCollection<BrowseWebItems> PagedMangaList { get; } = new();

    public ICommand ItemTappedCommand { get; }
    public ICommand SearchCommand { get; }

    public BrowseWebListPopup(BrowseVewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        ItemTappedCommand = new Command<BrowseWebItems>(async (item) => await OnItemTapped(item));
        SearchCommand = new Command<string>(async (query) => await LoadItems(query));

        BindingContext = this;

        _ = LoadItems(); // initial load
    }

    private async Task LoadItems(string searchQuery = null)
    {
        try
        {
            var client = new HttpClient();
            var html = await client.GetStringAsync(_viewModel.Url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.SelectNodes(_viewModel.ListXPath);

            if (nodes == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No items found with given XPath.", "OK");
                return;
            }

            var allItems = new List<BrowseWebItems>();

            foreach (var node in nodes)
            {
                var titleNode = node.SelectSingleNode(_viewModel.TitleXPath);
                var linkNode = node.SelectSingleNode(_viewModel.LinkXPath);
                var imageNode = node.SelectSingleNode(_viewModel.ImageXPath);

                if (titleNode == null) continue;

                var title = titleNode.InnerText.Trim();
                if (!string.IsNullOrWhiteSpace(searchQuery) && !title.ToLower().Contains(searchQuery.ToLower()))
                    continue;

                allItems.Add(new BrowseWebItems
                {
                    Title = title,
                    Url = linkNode?.GetAttributeValue("href", "#"),
                    ImageUrl = imageNode?.GetAttributeValue("data-src", "") ?? imageNode?.GetAttributeValue("src", "")
                });
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                PagedMangaList.Clear();
                foreach (var item in allItems)
                    PagedMangaList.Add(item);
            });
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task OnItemTapped(BrowseWebItems item)
    {
        Application.Current.MainPage.ShowPopup(new ComicInfoPopup(item));
    }
}
