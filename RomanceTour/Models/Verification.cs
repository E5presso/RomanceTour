using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RomanceTour.Models
{
	public class Verification
	{
		public int Id { get; set; }
		public string IpAddress { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}