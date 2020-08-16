using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Billing
    {
        public Billing()
        {
            ProductBilling = new HashSet<ProductBilling>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Bank { get; set; }
        public string Number { get; set; }

        public virtual ICollection<ProductBilling> ProductBilling { get; set; }
    }
}
