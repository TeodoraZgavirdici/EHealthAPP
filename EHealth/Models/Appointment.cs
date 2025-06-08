using EHealthApp.Data;
using SQLite;
namespace EHealthApp.Models
{
    /// <summary>
    /// Reprezintă un model pentru programările gestionate în aplicație, inclusiv pentru persistare în SQLite și notificări.
    /// </summary>
    public class Appointment : IRecord
    {
        /// <summary>
        /// Id unic pentru fiecare programare.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Titlul programării (e.g., "Consultație medicală").
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Descrierea detaliată a programării.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Data și ora programării.
        /// </summary>
        public DateTime AppointmentDate { get; set; }
    }
}