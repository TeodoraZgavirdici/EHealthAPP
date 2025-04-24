using SQLite;
using EHealthApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public Task<int> SavePrescriptionAsync(Prescription prescription)
        {
            return SaveAsync(prescription);
        }

        public Task<int> DeletePrescriptionAsync(Prescription prescription)
        {
            return DeleteAsync(prescription);
        }
    }


    /// <summary>
    /// Interfață comună pentru toate entitățile care necesită un ID unic.
    /// </summary>
    public interface IRecord
    {
        int Id { get; set; }
    }
}
