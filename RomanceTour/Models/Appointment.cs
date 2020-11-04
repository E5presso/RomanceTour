using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public enum AppointmentStatus
    {
        READY_TO_PAY,   // 결제 대기중
        CONFIRMED,      // 예약 완료
        CANCELED,       // 취소됨
    }
    public partial class Appointment
    {
        public Appointment()
        {
            Person = new HashSet<Person>();
        }

        public int Id { get; set; }
        public int DateSessionId { get; set; }
        public bool IsUserAppointment { get; set; }
        public DateTime TimeStamp { get; set; }
        public AppointmentStatus Status { get; set; }
        public int? UserId { get; set; }
        public string Password { get; set; }
        public string HashSalt { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string BillingName { get; set; }
        public string BillingBank { get; set; }
        public string BillingNumber { get; set; }
        public int Ammount { get; set; }
        public int Price { get; set; }
        public string Link { get; set; }

        public virtual DateSession DateSession { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Person> Person { get; set; }
    }
}