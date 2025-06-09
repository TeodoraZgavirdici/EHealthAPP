using EHealthApp.Data;
using SQLite;


namespace EHealthApp.Models
{
    public class MedicalDocument : IRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; } 
        public string FilePath { get; set; } 
        public DateTime DateAdded { get; set; } 
        public string Category { get; set; } 

    }
}
