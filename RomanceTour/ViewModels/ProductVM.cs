using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RomanceTour.ViewModels
{
	public class ProductVM
	{
		public int Id { get; set; }
		public int CategoryId { get; set; }
		public string CategoryName { get; set; }
		public string Thumbnail { get; set; }
		public string Title { get; set; }
		public string SubTitle { get; set; }
		public int Price { get; set; }
		public bool Confirmed { get; set; }
		public IEnumerable<DateTime> Available { get; set; }
		public DateTime FastAvailable { get; set; }
	}
}