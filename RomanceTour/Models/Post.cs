using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Post
    {
        public Post()
        {
            Comment = new HashSet<Comment>();
            Media = new HashSet<Media>();
        }

        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public int Like { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<Media> Media { get; set; }
    }
}
