using Application.Repositories;
using DataLayer.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// APIController used in combination with AJAX to manage how to add posts and delete them
namespace MatchBuddie.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostingController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IPostRepository _postRepository;
        public PostingController(UserManager<User> userManager,
                                  IPostRepository postRepository)
        {
            _userManager = userManager;
            _postRepository = postRepository;
        }

        // Save post that the user writes on another users wall using the API controller which is sent using ajax in the view
        [HttpPost]
        [Route("SavePost")]
        public async Task SavePost([FromBody] Post post)
        {
            var claims = HttpContext.User;
            var user = await _userManager.GetUserAsync(claims);
            var dtoPost = new Post
            {
                Content = post.Content,
                UserId = user.Id,
                ReciverId = post.ReciverId,
                Date = DateTime.Now
            };
            await _postRepository.SavePost(claims, dtoPost);

        }
        [HttpPost]
        [Route("DeletePost")]
        public async Task DeletePost(int id)
        {
            var claims = HttpContext.User;
            var post = _postRepository.GetPostByID(id);
            await _postRepository.DeletePost(claims, post);

        }

    }
}

