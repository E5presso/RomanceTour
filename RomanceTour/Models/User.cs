using RomanceTour.Middlewares.DataEncryption.Attributes;
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
            Log = new HashSet<Log>();
        }

        public int Id { get; set; }
        public DateTime LastLogin { get; set; }
        public bool AllowTermsAndConditions { get; set; }
        public bool AllowMarketingPromotions { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HashSalt { get; set; }
        public UserStatus Status { get; set; }
        [Encrypted] public string Name { get; set; }
        [Encrypted] public string Address { get; set; }
        [Encrypted] public string Phone { get; set; }
        public DateTime Birthday { get; set; }
        [Encrypted] public string BillingName { get; set; }
        [Encrypted] public string BillingBank { get; set; }
        [Encrypted] public string BillingNumber { get; set; }

        public virtual ICollection<Appointment> Appointment { get; set; }
        public virtual ICollection<Log> Log { get; set; }
    }
}
