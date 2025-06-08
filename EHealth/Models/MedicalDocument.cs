using EHealthApp.Data;
using SQLite;


namespace EHealthApp.Models
{
    public class MedicalDocument : IRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; } // Titlul documentului (ex: "Analize sânge")
        public string FilePath { get; set; } // Calea fișierului salvat (local)
        public DateTime DateAdded { get; set; } // Data adăugării documentului
    }
}
