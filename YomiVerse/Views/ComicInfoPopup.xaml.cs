using CommunityToolkit.Maui.Views;
using YomiVerse.Services;

namespace YomiVerse.Views;

public partial class ComicInfoPopup : Popup
{
    private readonly BrowseWebItems _item;
    private readonly DbService _dbService = new();

    public ComicInfoPopup(BrowseWebItems item)
    {
        InitializeComponent();
        _item = item;

        TitleLabel.Text = item.Title;
        DescLabel.Text = item.Description ?? "No description available.";
        ComicImage.Source = item.ImageUrl;

        CheckIfAlreadyExists();
    }

    private async void CheckIfAlreadyExists()
    {
        var libraryItems = await _dbService.GetLibraryEntriesAsync();
        var alreadyExists = libraryItems.Any(entry => entry.Title_Url == _item.Url);

        if (alreadyExists)
        {
            AddToLibraryButton.Text = "? Already in Library";
            AddToLibraryButton.IsEnabled = false;
        }
        else
        {
            AddToLibraryButton.Text = "? Add to Library";
            AddToLibraryButton.IsEnabled = true;
        }
    }

    private async void OnAddToLibraryClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(_item.ImageUrl))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Image URL is empty or null.", "OK");
                return;
            }

            byte[] bytes;
            using (var httpClient = new HttpClient())
            {
                try
                {
                    bytes = await httpClient.GetByteArrayAsync(_item.ImageUrl);
                }
                catch
                {
                    bytes = Array.Empty<byte>(); // fallback for image
                }
            }

            var entry = new LibraryList
            {
                TitleName = _item.Title,
                Title_Url = _item.Url,
                Title_Desc = _item.Description ?? "",
                Title_ImageBytes = bytes
            };

            await _dbService.AddLibraryItemAsync(entry);

            Microsoft.Maui.Controls.MessagingCenter.Send<object>(this, "LibraryUpdated");

            await Application.Current.MainPage.DisplayAlert("Success", "Added to Library", "OK");

            Close();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Crash", ex.ToString(), "OK");
        }
    }
}
