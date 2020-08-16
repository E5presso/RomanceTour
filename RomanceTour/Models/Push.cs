using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Push
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; }
        public string Group { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
