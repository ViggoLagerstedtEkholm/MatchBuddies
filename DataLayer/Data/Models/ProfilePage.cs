using System.Collections.Generic;

namespace DataLayer.Data.Models
{
    public class ProfilePage
    {
        public int ProfilePageID { get; set; }
        public string Owner { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
