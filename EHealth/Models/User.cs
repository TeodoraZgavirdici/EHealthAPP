using SQLite;

namespace EHealthApp.Data
{
    public class User: IRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100), Unique, NotNull]
        public string Username { get; set; }

        [MaxLength(100), NotNull]
        public string Password { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } // <--- ADĂUGĂ ACEASTĂ LINIE
        public string Name { get; set; }

    }
}
