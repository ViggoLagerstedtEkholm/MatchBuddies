using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Application.Repositories;
using DataLayer.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace MatchBuddie.Controllers
{
    [Authorize]
    public class ShowcaseUsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IFriendRepository _friendRepository;
        public ShowcaseUsersController(UserManager<User> userManager,
                                       IUserRepository userRepository,
                                       IFriendRepository friendRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _friendRepository = friendRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(string search)
        {
            var users = _userRepository.AllUsers;
            var claims = HttpContext.User;
            var user = await _userManager.GetUserAsync(claims);


            if (!String.IsNullOrEmpty(search))
            {
                users = users.Where(s => s.Firstname.ToLower().Contains(search.ToLower()) || s.Lastname.ToLower().Contains(search.ToLower())); 
            }

            return View(users);
        }
        [HttpPost]
        public async Task<ActionResult> Index()
        {
            var claims = HttpContext.User;
            var matchingUsers = await _userRepository.GetAllMatches(claims);

            return View(matchingUsers.ToList());
        }
        [HttpPost]
        [Route("ShowcaseUsers/AddFriend")]
        public IActionResult AddFriend(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            _friendRepository.SaveNewFriendship(id, userId);

            return Ok();
        }
    }
}

