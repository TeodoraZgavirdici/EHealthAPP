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

       
        public DateTime AppointmentDate { get; set; }
    }
}