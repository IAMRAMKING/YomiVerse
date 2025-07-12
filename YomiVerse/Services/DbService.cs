using SQLite;
using System.Collections.ObjectModel;

namespace YomiVerse.Services
{
    internal class DbService
    {
        private const string m_strDB_Name = "YomiVerse.db3";

        private readonly SQLiteAsyncConnection m_obSqlConnection;

        public DbService()
        {
            m_obSqlConnection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, m_strDB_Name));
            m_obSqlConnection.CreateTableAsync<LibraryList>();
        }

        public async Task<List<LibraryList>> GetLibraryListsAsync()
        {
            return await m_obSqlConnection.Table<LibraryList>().ToListAsync();
        }

        public async Task<ObservableCollection<LibraryList>> GetLibraryEntriesAsync()
        {
            var list = await m_obSqlConnection.Table<LibraryList>().ToListAsync();
            return new ObservableCollection<LibraryList>(list);
        }

        public async Task AddLibraryItemAsync(LibraryList item)
        {
            await m_obSqlConnection.InsertAsync(item);
        }

        public async Task UpdateImageOrUrlAsync(int id, byte[] newImageBytes = null, string newUrl = null)
        {
            var item = await m_obSqlConnection.FindAsync<LibraryList>(id);
            if (item != null)
            {
                if (newImageBytes != null)
                    item.Title_ImageBytes = newImageBytes;

                if (!string.IsNullOrEmpty(newUrl))
                    item.Title_Url = newUrl;

                await m_obSqlConnection.UpdateAsync(item);
            }
        }

        public async Task AddBrowseSourceAsync(BrowseSource source)
        {
            await m_obSqlConnection.CreateTableAsync<BrowseSource>();
            await m_obSqlConnection.InsertAsync(source);
        }

        public async Task<List<BrowseSource>> GetBrowseSourcesAsync()
        {
            await m_obSqlConnection.CreateTableAsync<BrowseSource>();
            return await m_obSqlConnection.Table<BrowseSource>().ToListAsync();
        }

        public async Task DeleteBrowseSourceAsync(int id)
        {
            await m_obSqlConnection.CreateTableAsync<BrowseSource>();
            var item = await m_obSqlConnection.FindAsync<BrowseSource>(id);
            if (item != null)
            {
                await m_obSqlConnection.DeleteAsync(item);
            }
        }
    }
}
