using EHealthApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHealthApp.Data
{
    public class AppDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public AppDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);

            _database.CreateTableAsync<User>().Wait();
            _database.CreateTableAsync<Appointment>().Wait();
            _database.CreateTableAsync<MedicalDocument>().Wait();
            _database.CreateTableAsync<Prescription>().Wait();
        }

       
        public Task<List<T>> GetAllAsync<T>() where T : new()
        {
            return _database.Table<T>().ToListAsync();
        }

       
        public Task<int> SaveAsync<T>(T item) where T : IRecord, new()
        {
            return item.Id != 0 ? _database.UpdateAsync(item) : _database.InsertAsync(item);
        }

        
        public Task<int> DeleteAsync<T>(T item) where T : IRecord, new()
        {
            return _database.DeleteAsync(item);
        }

    
        public Task<List<User>> GetUsersAsync() => GetAllAsync<User>();

        public Task<int> SaveUserAsync(User user) => SaveAsync(user);

        public Task<int> DeleteUserAsync(User user) => DeleteAsync(user);

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

     
        public Task<User> GetUserByEmailAndPassword(string email, string password)
        {
            return _database.Table<User>()
                .Where(u => u.Email == email && u.Password == password)  
                .FirstOrDefaultAsync();
        }

        public Task<List<Appointment>> GetAppointmentsAsync() => GetAllAsync<Appointment>();

        public Task<int> SaveAppointmentAsync(Appointment appointment) => SaveAsync(appointment);

        public Task<int> DeleteAppointmentAsync(Appointment appointment) => DeleteAsync(appointment);

        public Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            return _database.Table<Appointment>()
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<List<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return _database.Table<Appointment>()
                .Where(a => a.AppointmentDate >= start && a.AppointmentDate < end)
                .ToListAsync();
        }

        public Task<List<MedicalDocument>> GetMedicalDocumentsAsync() => GetAllAsync<MedicalDocument>();

        public Task<List<MedicalDocument>> GetMedicalDocumentsByCategoryAsync(string category)
        {
            return _database.Table<MedicalDocument>()
                .Where(d => d.Category == category)
                .OrderByDescending(d => d.DateAdded)
                .ToListAsync();
        }

        public Task<MedicalDocument> GetMedicalDocumentByFilePathAsync(string filePath)
        {
            return _database.Table<MedicalDocument>()
                .Where(d => d.FilePath == filePath)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveMedicalDocumentAsync(MedicalDocument document) => SaveAsync(document);

        public Task<int> DeleteMedicalDocumentAsync(MedicalDocument document) => DeleteAsync(document);

        public Task<List<Prescription>> GetPrescriptionsAsync() => GetAllAsync<Prescription>();

        public Task<int> SavePrescriptionAsync(Prescription prescription) => SaveAsync(prescription);

        public Task<int> DeletePrescriptionAsync(Prescription prescription) => DeleteAsync(prescription);
    }

    public interface IRecord
    {
        int Id { get; set; }
    }
}
