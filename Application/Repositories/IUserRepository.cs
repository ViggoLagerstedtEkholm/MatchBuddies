using DataLayer.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> AllUsers { get; }
        User GetUserByID(string Id);
        User GetUserByEmail(string email);
        Task SaveUser(ClaimsPrincipal claims, User user);
        Task SaveImage(ClaimsPrincipal claims, User user);
        Task SaveInterest(ClaimsPrincipal claims, Interests interest);
        Task DeleteInterest(ClaimsPrincipal claims, Interests interest);
        void SaveVisit(ProfilePageUser pageVisit);
        Task<List<User>> GetAllMatches(ClaimsPrincipal claims);
        Task DeleteAccount(ClaimsPrincipal claims);
        Task HideAccount(ClaimsPrincipal claims);
        ProfilePage GetProfilePage(string id);
        IQueryable<object> GetLatestVisitors(User user);


    }
}
