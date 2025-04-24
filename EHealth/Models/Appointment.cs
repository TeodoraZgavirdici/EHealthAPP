using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHealthApp.Data;
using SQLite;


namespace EHealthApp.Models
{
    public class Appointment : IRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } 

        public string Title { get; set; } 
        public string Description { get; set; } 
        public DateTime Date { get; set; } 
    }
}
