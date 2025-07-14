using CommunityToolkit.Maui.Views;
using HtmlAgilityPack;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Input;
using YomiVerse.Services;
using YomiVerse.Views;

namespace YomiVerse.ViewModels
{
    public class BrowseVewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Url { get; set; }
        public string ListXPath { get; set; }
        public string TitleXPath { get; set; }
        public string LinkXPath { get; set; }
        public string ImageXPath { get; set; }

        public ICommand BrowseCommand { get; }
        public ICommand SaveSourceCommand { get; }
        public ICommand LoadSourceCommand { get; }
        public ICommand DeleteSourceCommand { get; }

        public ObservableCollection<BrowseSource> SavedSources { get; set; } = new();

        public BrowseVewModel()
        {
            BrowseCommand = new Command(async () => await BrowseAsync());
            SaveSourceCommand = new Command(async () => await SaveSourceAsync());
            LoadSourceCommand = new Command<BrowseSource>((src) => LoadSource(src));
            DeleteSourceCommand = new Command<BrowseSource>(async (src) => await DeleteSourceAsync(src));

        }

        public async Task RefreshSourcesOnLoad()
        {
            await LoadSavedSources();
        }

        private async Task BrowseAsync()
        {
            try
            {
                var client = new HttpClient();
                var html = await client.GetStringAsync(Url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var items = new List<BrowseWebItems>();
                var nodes = doc.DocumentNode.SelectNodes(ListXPath);

                if (nodes == null || nodes.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No manga list found. Check your XPath.", "OK");
                    return;
                }

                foreach (var node in nodes)
                {
                    var titleNode = node.SelectSingleNode(TitleXPath);
                    var linkNode = node.SelectSingleNode(LinkXPath);
                    var imageNode = node.SelectSingleNode(ImageXPath);

                    if (titleNode != null)
                    {
                        items.Add(new BrowseWebItems
                        {
                            Title = titleNode.InnerText.Trim(),
                            Url = linkNode?.GetAttributeValue("href", "#"),
                            ImageUrl = imageNode?.GetAttributeValue("data-src", "") ?? imageNode?.GetAttributeValue("src", "")
                        });
                    }
                }

                if (items.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Notice", "No items parsed. Check XPath selectors.", "OK");
                    return;
                }

                //var popup = new BrowseWebListPopup(items);
                var popup = new BrowseWebListPopup(this); // Pass the ViewModel

                Application.Current.MainPage.ShowPopup(popup);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task SaveSourceAsync()
        {
            var db = new DbService();

            // Check for duplicate based on URL and XPath combination
            var existingSources = await db.GetBrowseSourcesAsync();
            bool isDuplicate = existingSources.Any(src =>
                src.Url == Url &&
                src.ListXPath == ListXPath &&
                src.TitleXPath == TitleXPath &&
                src.LinkXPath == LinkXPath &&
                src.ImageXPath == ImageXPath);

            if (isDuplicate)
            {
                await Application.Current.MainPage.DisplayAlert("Duplicate", "This source already exists.", "OK");
                return;
            }

            var source = new BrowseSource
            {
                Url = Url,
                ListXPath = ListXPath,
                TitleXPath = TitleXPath,
                LinkXPath = LinkXPath,
                ImageXPath = ImageXPath
            };

            await db.AddBrowseSourceAsync(source);
            await LoadSavedSources();
        }

        private async Task DeleteSourceAsync(BrowseSource source)
        {
            var db = new DbService();
            await db.DeleteBrowseSourceAsync(source.Id);
            await LoadSavedSources();
        }

        private async Task LoadSavedSources()
        {
            var db = new DbService();
            var list = await db.GetBrowseSourcesAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                SavedSources.Clear();
                foreach (var src in list)
                    SavedSources.Add(src);
            });
        }

        private void LoadSource(BrowseSource src)
        {
            Url = src.Url;
            ListXPath = src.ListXPath;
            TitleXPath = src.TitleXPath;
            LinkXPath = src.LinkXPath;
            ImageXPath = src.ImageXPath;

            OnPropertyChanged(nameof(Url));
            OnPropertyChanged(nameof(ListXPath));
            OnPropertyChanged(nameof(TitleXPath));
            OnPropertyChanged(nameof(LinkXPath));
            OnPropertyChanged(nameof(ImageXPath));
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
