using System.Linq;
using DataLayer.Data;
using DataLayer.Data.Models;
//Repository regarding friendships
namespace Application.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly MatchBuddieContext _dbContext;
        public FriendRepository(MatchBuddieContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void SaveNewFriendship(string friendID, string userID)
        {
            var currentUser = _dbContext.Users.FirstOrDefault(u => u.Id == userID);
            var friendUser = _dbContext.Users.FirstOrDefault(u => u.Id == friendID);

            Friendship Friendship = new Friendship
            {
                ApplicationUser = currentUser,
                Friend = friendUser,
                Status = FriendRequestFlag.None
            };

            _dbContext.Requests.Add(Friendship);
            _dbContext.SaveChanges();
        }
        public void AcceptFriendship(string ApplicationUserId, string FriendId)
        {
            var friendship = _dbContext.Requests.Find(ApplicationUserId, FriendId);
            friendship.Status = FriendRequestFlag.Approved;

            _dbContext.SaveChanges();
        }
        public void DeclineFriendship(string ApplicationUserId, string FriendId)
        {
            var friendship = _dbContext.Requests.Find(ApplicationUserId, FriendId);
            friendship.Status = FriendRequestFlag.Rejected;

            _dbContext.SaveChanges();
        }
        public Friendship GetFriendship(string FriendID, string applicationID)
        {
            var friendship = _dbContext.Requests.Find(applicationID, FriendID);

            return friendship;
        }

        // Recievs two IDs User A sends to User B or User B sends to User A
        // The method checks both combinations and updates the related column(Category column)
        public void SaveFriendshipCategory(string senderID, string reciverID, string newCategory)
        {

            var kindOfrelationship = GetFriendship(senderID, reciverID);
            if (kindOfrelationship == null)
            {
                var friendship = GetFriendship(reciverID, senderID);

                switch (newCategory)
                {
                    case "Priority":
                        friendship.RelationshipTypeUserA = TypeOfFriendship.Priority;
                        break;
                    case "Standard":
                        friendship.RelationshipTypeUserA = TypeOfFriendship.Standard;
                        break;
                    case "Low":
                        friendship.RelationshipTypeUserA = TypeOfFriendship.Low;
                        break;
                }
            }
            else
            {
                var friendship = GetFriendship(senderID, reciverID);

                switch (newCategory)
                {
                    case "Priority":
                        friendship.RelationshipTypeUserB = TypeOfFriendship.Priority;
                        break;
                    case "Standard":
                        friendship.RelationshipTypeUserB = TypeOfFriendship.Standard;
                        break;
                    case "Low":
                        friendship.RelationshipTypeUserB = TypeOfFriendship.Low;
                        break;
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
