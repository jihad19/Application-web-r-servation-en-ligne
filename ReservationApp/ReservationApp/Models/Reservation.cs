using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationApp.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public virtual ICollection<TypeReservation> TypeReservation { get; set; }
        public virtual User User { get; set; }


    }
}
