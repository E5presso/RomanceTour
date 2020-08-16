using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RomanceTour.Models;

namespace RomanceTour.ViewModels
{
	public class DateSessionVM
	{
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateSessionStatus Status { get; set; }
        public int Reserved { get; set; }
        public int Paid { get; set; }
        public int Sales { get; set; }
    }
}