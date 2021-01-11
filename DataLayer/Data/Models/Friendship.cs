
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Data.Models
{
    public class Friendship
    {
        public string ApplicationUserId { get; set; }
        public virtual User ApplicationUser { get; set; }
        public string FriendId { get; set; }
        public virtual User Friend { get; set; }
        public FriendRequestFlag Status { get; set; }
        public TypeOfFriendship RelationshipTypeUserA { get; set; }
        public TypeOfFriendship RelationshipTypeUserB { get; set; }
        [NotMapped]
        public bool Approved => Status == FriendRequestFlag.Approved;
    }
}
