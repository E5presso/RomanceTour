using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RomanceTour.Models;

namespace RomanceTour.ViewModels
{
	public class ManageAppointmentVM
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Ammount { get; set; }
		public bool IsUser { get; set; }
		public AppointmentStatus Status { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public int Price { get; set; }
		public string BillingName { get; set; }
		public string BillingBank { get; set; }
		public string BillingNumber { get; set; }
	}
}