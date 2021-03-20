using Application.Repositories;
using DataLayer.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
// Controller linked to the friend view
namespace MatchBuddie.Controllers
{
    [Authorize]
    public class FriendController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IFriendRepository _friendRepository;

        public FriendController(UserManager<User> manager,
                                IFriendRepository friendRepository)
        {
            _userManager = manager;
            _friendRepository = friendRepository;
        }

        public async Task<IActionResult> Index()
        {
            var claims = HttpContext.User;
            var user = await _userManager.GetUserAsync(claims);

            return View(user);
        }

        [HttpPost]
        public void Accept(string ApplicationUserId, string FriendId)
        {
            _friendRepository.AcceptFriendship(ApplicationUserId, FriendId);
        }

        [HttpPost]
        public void Decline(string ApplicationUserId, string FriendId)
        {
            _friendRepository.DeclineFriendship(ApplicationUserId, FriendId);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateReceivedRequest()
        {
            var claims = HttpContext.User;
            var user = await _userManager.GetUserAsync(claims);
            return PartialView("_ReceivedRequest", user);
        }

    }
}
