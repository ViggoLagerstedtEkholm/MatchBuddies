using DataLayer.Data.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IPostRepository
    {
        public Post GetPostByID(int id);
        //public List<Post> GetPostsByUserID(string id);
        Task SavePost(ClaimsPrincipal claims, Post post);
        Task DeletePost(ClaimsPrincipal claims, Post post);

    }
}
