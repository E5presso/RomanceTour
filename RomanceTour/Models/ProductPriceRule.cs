using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class ProductPriceRule
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int PriceRuleId { get; set; }

        public virtual PriceRule PriceRule { get; set; }
        public virtual Product Product { get; set; }
    }
}
