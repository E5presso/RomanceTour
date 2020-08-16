using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Host
    {
        public Host()
        {
            ProductHost = new HashSet<ProductHost>();
        }

        public int Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string HostName { get; set; }
        public string HostPhone { get; set; }
        public string HostBank { get; set; }
        public string HostBillingNumber { get; set; }

        public virtual ICollection<ProductHost> ProductHost { get; set; }
    }
}
