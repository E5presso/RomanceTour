using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Log
    {
        public long Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string IpAddress { get; set; }
        public bool IsAdministrator { get; set; }
        public int? UserId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Parameter { get; set; }

        public virtual User User { get; set; }
    }
}
