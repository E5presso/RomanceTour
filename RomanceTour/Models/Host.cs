using RomanceTour.Middlewares.DataEncryption.Attributes;
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
        [Encrypted] public string Name { get; set; }
        [Encrypted] public string Address { get; set; }
        [Encrypted] public string HostName { get; set; }
        [Encrypted] public string HostPhone { get; set; }
        [Encrypted] public string HostBank { get; set; }
        [Encrypted] public string HostBillingNumber { get; set; }

        public virtual ICollection<ProductHost> ProductHost { get; set; }
    }
}
