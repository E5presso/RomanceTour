using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RomanceTour.ViewModels
{
	public class ProductFilterVM
	{
		public int? Sorting { get; set; }
		public int? FromPrice { get; set; }
		public int? ToPrice { get; set; }
		public DateTime? Date { get; set; }
		public bool? Confirmed { get; set; }
		public string Keyword { get; set; }
	}
}