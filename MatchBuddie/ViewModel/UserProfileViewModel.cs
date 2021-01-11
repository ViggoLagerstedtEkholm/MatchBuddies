using DataLayer.Data.Models;
using System.Collections.Generic;

namespace MatchBuddie.ViewModel
{
    public class UserProfileViewModel
    {
        public User user { get; set; }
        public IEnumerable<Interests> Interests { get; set; } = new List<Interests>();
        public IEnumerable<Post> posts { get; set; } = new List<Post>();
        public IEnumerable<ProfilePageUser> fiveLatestVisitors { get; set; } = new List<ProfilePageUser>();
        public IEnumerable<RelationType> typeOfRelation { get; set; } = new List<RelationType>();
    }
}
