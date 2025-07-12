using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using YomiVerse.Services;

namespace YomiVerse.ViewModels;

public class LibraryViewModel : INotifyPropertyChanged
{
    public ObservableCollection<LibraryList> LibraryItems { get; set; } = new();

    private readonly DbService _dbService = new();

    public LibraryViewModel()
    {
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

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
