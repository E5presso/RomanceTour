using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace RomanceTour.ViewModels
{
	public class PostVM
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string SubTitle { get; set; }
		public int CategoryId { get; set; }
		public IFormFile Thumbnail { get; set; }
		public int Price { get; set; }
		public List<int> PriceRules { get; set; }
		public List<int> Departures { get; set; }
		public List<int> Hosts { get; set; }
		public List<int> Billings { get; set; }
		public List<DateTime> Appointments { get; set; }
		public string Form { get; set; }
	}
}