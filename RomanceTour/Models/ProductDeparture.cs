using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class ProductDeparture
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int DepartureId { get; set; }

        public virtual Departure Departure { get; set; }
        public virtual Product Product { get; set; }
    }
}
