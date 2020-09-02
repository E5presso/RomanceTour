using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public enum UserStatus
    {
        GREEN,  // 정상 계정
        YELLOW, // 경고 계정
        RED,    // 정지 계정
        GREY    // 휴면 계정      
    }
    public partial class User
    {
        public User()
        {
            Appointment = new HashSet<Appointment>();
            Help = new HashSet<Help>();
            Log = new HashSet<Log>();
            Review = new HashSet<Review>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HashSalt { get; set; }
        public UserStatus Status { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime Birthday { get; set; }
        public string BillingName { get; set; }
        public string BillingBank { get; set; }
        public string BillingNumber { get; set; }

        public virtual ICollection<Appointment> Appointment { get; set; }
        public virtual ICollection<Help> Help { get; set; }
        public virtual ICollection<Log> Log { get; set; }
        public virtual ICollection<Review> Review { get; set; }
    }
}
