using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RomanceTour.ViewModels
{
	public class SessionFilterVM
	{
		public int? FromAppointment { get; set; }
		public int? ToAppointment { get; set; }
		public int? FromPaid { get; set; }
		public int? ToPaid { get; set; }
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }
	}
}