using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Person
    {
        public Person()
        {
            Option = new HashSet<Option>();
        }

        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int DepartureId { get; set; }
        public int Ammount { get; set; }

        public virtual Appointment Appointment { get; set; }
        public virtual Departure Departure { get; set; }
        public virtual ICollection<Option> Option { get; set; }
    }
}
