using DataLayer.Data;
using DataLayer.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
// Repository regarding post users make and get from other users
namespace Application.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly MatchBuddieContext _dbContext;
        private readonly IUserRepository _userRepository;

        public PostRepository(UserManager<User> userManager,
                              MatchBuddieContext dbContext,
                              IUserRepository userRepository)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _userRepository = userRepository;

        }
 
        public async Task DeletePost(ClaimsPrincipal claims, Post post)
        {
            var _user = await _userManager.GetUserAsync(claims);
            _dbContext.Posts.Remove(post);
            await _userManager.UpdateAsync(_user);

        }
        public async Task SavePost(ClaimsPrincipal claims, Post post)
        {
            var _user = await _userManager.GetUserAsync(claims);
            _dbContext.Posts.Add(post);
            await _userManager.UpdateAsync(_user);

        }
        public Post GetPostByID(int id)
        {
            return _dbContext.Posts.Find(id);
        }
    }
}
