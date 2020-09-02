using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Verification
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
