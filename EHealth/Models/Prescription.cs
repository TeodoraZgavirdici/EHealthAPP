using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHealthApp.Data;
using SQLite;


namespace EHealthApp.Models
{
    public class Prescription : IRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string MedicationName { get; set; } // Numele medicamentului
        public string Dosage { get; set; } // Doza prescrisă
        public string Instructions { get; set; } // Instrucțiuni de administrare
        public DateTime DatePrescribed { get; set; } // Data rețetei
    }
}
