
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Data.Models
{
    public class Post
    {
        public virtual int PostID { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string Content { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string ReciverId { get; set; }
        public virtual User Reciver { get; set; }
        public virtual DateTime Date { get; set; }
    }
}
