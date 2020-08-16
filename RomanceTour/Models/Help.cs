using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Help
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Ipaddress { get; set; }
        public int? UserId { get; set; }
        public string Command { get; set; }
        public string Data { get; set; }

        public virtual User User { get; set; }
    }
}
