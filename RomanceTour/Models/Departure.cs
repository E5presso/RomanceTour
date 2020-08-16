using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Departure
    {
        public Departure()
        {
            Person = new HashSet<Person>();
            ProductDeparture = new HashSet<ProductDeparture>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public float? X { get; set; }
        public float? Y { get; set; }

        public virtual ICollection<Person> Person { get; set; }
        public virtual ICollection<ProductDeparture> ProductDeparture { get; set; }
    }
}
