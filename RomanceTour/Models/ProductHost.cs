using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class ProductHost
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int HostId { get; set; }

        public virtual Host Host { get; set; }
        public virtual Product Product { get; set; }
    }
}
