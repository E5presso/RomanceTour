using System.Collections.Generic;

namespace RomanceTour.ViewModels
{
	public class AppointmentPeopleVM
	{
		public int Price { get; set; }
		public int Departure { get; set; }
		public List<int> Options { get; set; }
		public int Ammount { get; set; }
	}
}