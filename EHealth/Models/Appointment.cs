using System;
using SQLite; // Pentru utilizarea atributele SQLite (e.g., [PrimaryKey], [AutoIncrement])
using EHealthApp.Data;


namespace EHealthApp.Models
{
    /// <summary>
    /// Reprezintă un model pentru programările gestionate în aplicație.
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
        /// Data programării.
        /// </summary>
        public DateTime AppointmentDate { get; set; }
    }
}
