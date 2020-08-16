using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Media
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string Path { get; set; }

        public virtual Post Post { get; set; }
    }
}
