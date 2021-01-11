using System.Collections.Generic;

namespace DataLayer.Data.Models
{
    public class Interests
    {
        public int InterestsID { get; set; }
        public string Interest { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
