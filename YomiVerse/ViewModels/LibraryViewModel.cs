using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using YomiVerse.Services;

namespace YomiVerse.ViewModels;

public class LibraryViewModel : INotifyPropertyChanged
{
    public ObservableCollection<LibraryList> LibraryItems { get; set; } = new();
    public ObservableCollection<LibraryList> SelectedItems { get; set; } = new();

    public ICommand ComicTappedCommand { get; }
    public ICommand DeleteSelectedCommand { get; }


    private readonly DbService _dbService = new();

    public LibraryViewModel()
    {
        ComicTappedCommand = new Command<LibraryList>(OnComicTapped);
        DeleteSelectedCommand = new Command(async () => await DeleteSelectedAsync());
        LoadItemsFromDb();

        // Subscribe for update notification
        Microsoft.Maui.Controls.MessagingCenter.Subscribe<object>(this, "LibraryUpdated", async (sender) =>
        {
            await LoadItemsFromDb();
        });
        
    }

    private async Task LoadItemsFromDb()
    {
        var items = await _dbService.GetLibraryEntriesAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            LibraryItems.Clear();
            foreach (var item in items)
                LibraryItems.Add(item);
        });
    }

    private async Task DeleteSelectedAsync()
    {
        if (SelectedItems.Count == 0)
            return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Confirm",
            $"Delete {SelectedItems.Count} selected items?",
            "Yes", "No");

        if (!confirm)
            return;

        foreach (var item in SelectedItems.ToList())  // Use ToList to avoid modifying during enumeration
        {
            await _dbService.DeleteLibraryEntryAsync(item.ID);
            LibraryItems.Remove(item);
        }

        SelectedItems.Clear();
    }

    private async void OnComicTapped(LibraryList item)
    {
        // You can show a popup here or navigate to a details page.
        await Application.Current.MainPage.DisplayAlert("Comic Tapped", item.TitleName, "OK");

        // Example (popup): await Application.Current.MainPage.ShowPopupAsync(new ComicDetailsPopup(item));
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
