using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class ProductBilling
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int BillingId { get; set; }

        public virtual Billing Billing { get; set; }
        public virtual Product Product { get; set; }
    }
}
