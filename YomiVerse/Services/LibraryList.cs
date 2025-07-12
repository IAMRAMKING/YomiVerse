using SQLite;
using System.ComponentModel.DataAnnotations;

namespace YomiVerse.Services
{
    [Table("LibraryList")]
    public class LibraryList
    {
        [PrimaryKey, AutoIncrement]
        [Display(AutoGenerateField = false)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("Title_Name")]
        public string TitleName { get; set; }

        [Column("Title_Url")]
        public string Title_Url { get; set; }

        [Column("Title_Des")]
        public string Title_Desc { get; set; }

        [Column("Title_Image")]
        public byte[] Title_ImageBytes { get; set; }
    }
}
