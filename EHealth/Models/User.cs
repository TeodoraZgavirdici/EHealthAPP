using SQLite;

namespace EHealthApp.Data
{
  public class User : IRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Email { get; set; }

        [NotNull]
        public string Username { get; set; }


        [NotNull]
        public string Password { get; set; }

        public string FullName { get; set; }  // opțional, dar clar tip string
    }

}
