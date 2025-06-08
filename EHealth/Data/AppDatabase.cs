using EHealthApp.Models;
using SQLite;

namespace EHealthApp.Data
{
    public class AppDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        /// <summary>
        /// Constructor: Inițializează conexiunea și creează tabelele necesare.
        /// </summary>
        public AppDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);

            // Crearea tabelelor pentru toate tipurile de entități
            _database.CreateTableAsync<User>().Wait();
            _database.CreateTableAsync<Appointment>().Wait();
            _database.CreateTableAsync<MedicalDocument>().Wait();
            _database.CreateTableAsync<Prescription>().Wait();
        }

        // ------------------ METODE GENERICE ------------------

        /// <summary>
        /// Returnează toate intrările pentru o anumită entitate.
        /// </summary>
        public Task<List<T>> GetAllAsync<T>() where T : new()
        {
            return _database.Table<T>().ToListAsync();
        }

        /// <summary>
        /// Salvează sau actualizează o entitate în baza de date.
        /// </summary>
        public Task<int> SaveAsync<T>(T item) where T : IRecord, new()
        {
            return item.Id != 0 ? _database.UpdateAsync(item) : _database.InsertAsync(item);
        }

        /// <summary>
        /// Șterge o entitate din baza de date.
        /// </summary>
        public Task<int> DeleteAsync<T>(T item) where T : IRecord, new()
        {
            return _database.DeleteAsync(item);
        }

        // ------------------ METODE SPECIFICE ------------------

        // Users
        public Task<List<User>> GetUsersAsync()
        {
            return GetAllAsync<User>();
        }

        public Task<int> SaveUserAsync(User user)
        {
            return SaveAsync(user);
        }

        public Task<int> DeleteUserAsync(User user)
        {
            return DeleteAsync(user);
        }

        // Appointments
        public Task<List<Appointment>> GetAppointmentsAsync()
        {
            return GetAllAsync<Appointment>();
        }

        public Task<int> SaveAppointmentAsync(Appointment appointment)
        {
            return SaveAsync(appointment);
        }

        public Task<int> DeleteAppointmentAsync(Appointment appointment)
        {
            return DeleteAsync(appointment);
        }

        /// <summary>
        /// Găsește o programare după ID.
        /// </summary>
        public Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            return _database.Table<Appointment>()
            .Where(a => a.Id == id)
            .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Găsește programările programate pentru o anumită zi.
        /// </summary>
        public Task<List<Appointment>> GetAppointmentsByDateAsync(System.DateTime date)
        {
            // SQLite-net nu suportă a.AppointmentDate.Date direct!
            var start = date.Date;
            var end = date.Date.AddDays(1);
            return _database.Table<Appointment>()
                .Where(a => a.AppointmentDate >= start && a.AppointmentDate < end)
                .ToListAsync();
        }

        // MedicalDocuments
        public Task<List<MedicalDocument>> GetMedicalDocumentsAsync()
        {
            return GetAllAsync<MedicalDocument>();
        }

        public Task<int> SaveMedicalDocumentAsync(MedicalDocument document)
        {
            return SaveAsync(document);
        }

        public Task<int> DeleteMedicalDocumentAsync(MedicalDocument document)
        {
            return DeleteAsync(document);
        }

        // Prescriptions
        public Task<List<Prescription>> GetPrescriptionsAsync()
        {
            return GetAllAsync<Prescription>();
        }
        public Task<User> GetUserByUsernameAsync(string username)
        {
            return _database.Table<User>()
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();
        }
        public Task<User> GetUserByEmailAsync(string email)
        {
            return _database.Table<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }
        public Task<int> SavePrescriptionAsync(Prescription prescription)
        {
            return SaveAsync(prescription);
        }

        public Task<int> DeletePrescriptionAsync(Prescription prescription)
        {
            return DeleteAsync(prescription);
        }
    }

    public interface IRecord
    {
        int Id { get; set; }
    }
}