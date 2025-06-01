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

        public string MedicationName { get; set; } 
        public string Dosage { get; set; } 
        public string Instructions { get; set; } 
        public DateTime DatePrescribed { get; set; } 
    }
}
