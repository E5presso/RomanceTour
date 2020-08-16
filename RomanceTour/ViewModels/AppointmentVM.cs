using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RomanceTour.ViewModels
{
	public class AppointmentVM
	{
		public int Id { get; set; }
		public bool IsUserAppointment { get; set; }
		public string DateString { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public string Password { get; set; }
		public string BillingName { get; set; }
		public string BillingBank { get; set; }
		public string BillingNumber { get; set; }
		public DateTime Date => DateTime.ParseExact(DateString, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);

		public List<AppointmentPeopleVM> People { get; set; }
	}
}