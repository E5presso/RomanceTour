using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Error
    {
        public long Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
