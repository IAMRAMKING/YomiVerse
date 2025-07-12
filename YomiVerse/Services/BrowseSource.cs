using SQLite;

namespace YomiVerse.Services
{
    [Table("BrowseSource")]
    public class BrowseSource
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Url { get; set; }
        public string ListXPath { get; set; }
        public string TitleXPath { get; set; }
        public string LinkXPath { get; set; }
        public string ImageXPath { get; set; }
    }
}
