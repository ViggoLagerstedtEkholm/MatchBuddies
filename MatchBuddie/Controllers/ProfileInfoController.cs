using Application.Repositories;
using DataLayer.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MatchBuddie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileInfoController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        public ProfileInfoController(IUserRepository userRepository,
                                     UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        // Method that gets the five latest visitors of a page

        [HttpGet]
        [Route("GetVisitors")]
        public async Task<JsonResult> GetFiveLastVisitors()
        {
            var claims = HttpContext.User;
            var user = await _userManager.GetUserAsync(claims);

            return new JsonResult(_userRepository.GetLatestVisitors(user).ToList().Take(5)); // Using Linq method Take() to only get the latest five in the list 
        }

    }
}

