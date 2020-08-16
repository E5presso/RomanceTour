using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public enum DateSessionStatus
    {
        AVAILABLE,  // 모집 중
        APPROVED,   // 출발확정
        FULLED,     // 마감
        CANCELED    // 취소
    }
    public partial class DateSession
    {
        public DateSession()
        {
            Appointment = new HashSet<Appointment>();
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime Date { get; set; }
        public DateSessionStatus Status { get; set; }
        public int Reserved { get; set; }
        public int Paid { get; set; }
        public int Sales { get; set; }

        public virtual Product Product { get; set; }
        public virtual ICollection<Appointment> Appointment { get; set; }
    }
}
