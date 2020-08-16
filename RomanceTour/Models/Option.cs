using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Option
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int PriceRuleId { get; set; }

        public virtual Person Person { get; set; }
        public virtual PriceRule PriceRule { get; set; }
    }
}
