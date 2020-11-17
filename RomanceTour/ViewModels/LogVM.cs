using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RomanceTour.ViewModels
{
	public class LogVM
	{
        public DateTime TimeStamp { get; set; }
        public string IpAddress { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}