using CommunityToolkit.Maui.Views;
using YomiVerse.Services;

namespace YomiVerse.Views;

public partial class ComicInfoPopup : Popup
{
    private readonly BrowseWebItems _item;

    public ComicInfoPopup(BrowseWebItems item)
    {
        InitializeComponent();
        _item = item;
        TitleLabel.Text = item.Title;
        DescLabel.Text = item.Description ?? "No description available.";
        ComicImage.Source = item.ImageUrl;
    }

    private async void OnAddToLibraryClicked(object sender, EventArgs e)
    {
        try
        {
            using var httpClient = new HttpClient();
            var bytes = await httpClient.GetByteArrayAsync(_item.ImageUrl);

            var db = new DbService();
            var entry = new LibraryList
            {
                TitleName = _item.Title,
                Title_Url = _item.Url,
                Title_Desc = _item.Description ?? "",
                Title_ImageBytes = bytes
            };

            await db.AddLibraryItemAsync(entry);

            Microsoft.Maui.Controls.MessagingCenter.Send<object>(this, "LibraryUpdated");

            await Application.Current.MainPage.DisplayAlert("Success", "Added to Library", "OK");
            Close();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

}
