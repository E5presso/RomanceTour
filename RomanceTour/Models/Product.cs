using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Product
    {
        public Product()
        {
            DateSession = new HashSet<DateSession>();
            ProductBilling = new HashSet<ProductBilling>();
            ProductDeparture = new HashSet<ProductDeparture>();
            ProductHost = new HashSet<ProductHost>();
            ProductPriceRule = new HashSet<ProductPriceRule>();
            Review = new HashSet<Review>();
        }

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Thumbnail { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int Price { get; set; }
        public string Form { get; set; }
        public int Rating { get; set; }
        public bool Visible { get; set; }
        public bool Expose { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<DateSession> DateSession { get; set; }
        public virtual ICollection<ProductBilling> ProductBilling { get; set; }
        public virtual ICollection<ProductDeparture> ProductDeparture { get; set; }
        public virtual ICollection<ProductHost> ProductHost { get; set; }
        public virtual ICollection<ProductPriceRule> ProductPriceRule { get; set; }
        public virtual ICollection<Review> Review { get; set; }
    }
}