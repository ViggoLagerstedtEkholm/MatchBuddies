using DataLayer.Data.Models;

namespace Application.Repositories
{
    public interface IFriendRepository
    {
        void SaveFriendshipCategory(string applicationID, string FriendID, string newCategory);
        void SaveNewFriendship(string friendID, string UserID);
        void AcceptFriendship(string appUser, string friendUser);
        void DeclineFriendship(string appUser, string friendUser);
        Friendship GetFriendship(string FriendID, string applicationID);

    }
}
