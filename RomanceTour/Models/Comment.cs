using System;
using System.Collections.Generic;

namespace RomanceTour.Models
{
    public partial class Comment
    {
        public Comment()
        {
            InverseCommentNavigation = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public int? CommentId { get; set; }
        public string Message { get; set; }

        public virtual Comment CommentNavigation { get; set; }
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Comment> InverseCommentNavigation { get; set; }
    }
}
