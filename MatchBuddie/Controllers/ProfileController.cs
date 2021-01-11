using Application;
using Application.Repositories;
using DataLayer.Data.Models;
using MatchBuddie.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

// Controller linked to the Profile view, the most extensive of the controllers as Profile contains method for a users own page aswell as the one they visits

namespace MatchBuddie.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IInterestRepository _interestRepository;
        private readonly UserManager<User> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IFriendRepository _friendRepository;
        private readonly SignInManager<User> _signInManager;

        public ProfileController(IUserRepository userRepository,
            IInterestRepository interestRepository,
            UserManager<User> userManager,
            ICategoryRepository categoryRepository,
            IFriendRepository friendRepository,
            SignInManager<User> signInManager)
        {
            _userRepository = userRepository;
            _interestRepository = interestRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _friendRepository = friendRepository;
            _signInManager = signInManager;
        }

        private async Task<User> GetUser()
        {
            var claims = HttpContext.User;
            var user = _userManager.GetUserAsync(claims);
            return await user;
        }
        // Made a method of the viewmodel to remove dublicated code
        private async Task<UserProfileViewModel> GetUserViewModel()
        {
            var user = await GetUser();
            
                var userProfileViewModel = new UserProfileViewModel();
                user.Interests = _interestRepository.UsersInterest(user.Id);
                userProfileViewModel.user = user;
                userProfileViewModel.Interests = _interestRepository.AllInterests;
                userProfileViewModel.typeOfRelation = _categoryRepository.AllCategories;

                return userProfileViewModel;
        
        }
        public async Task<IActionResult> Index()
        {
            return View(await GetUserViewModel());
        }
        public async Task<IActionResult> About()
        {
            return View(await GetUserViewModel());
        }
        public async Task<IActionResult> Friends()
        {
            return View(await GetUserViewModel());
        }

        // This method calls for the method GetAllMatches and Check if the profile you are visiting is a match. 
        [HttpPost]
        public async Task<IActionResult> CheckCompatability(string profileID)
        {
            var claims = HttpContext.User;
            var matchingUsers = await _userRepository.GetAllMatches(claims);
            var isCompatible = false;

            //Check if the user on the profile is in the list of all matching user of the person logged in.
            foreach (User aUser in matchingUsers)
            {
                if (aUser.Id == profileID)
                {
                    isCompatible = true;
                }
            }

            return Json(isCompatible);
        }

        public async Task<IActionResult> VisitUser(string id)
        {
            var visitedUser = _userRepository.GetUserByID(id);
            var user = await GetUser();

            if (visitedUser == null)
                return NotFound();

            if (visitedUser.Id.Equals(user.Id) || !visitedUser.isActive)
            {
                return RedirectToAction("Index", "Profile", new { area = "" });
            }

            await SaveVisits(id);
            return View(visitedUser);
        }

        // Method that Saves the visit a users makes on another users profile page
        private async Task SaveVisits(string id)
        {
            var user = await GetUser();
            var page = _userRepository.GetProfilePage(id);
            var dtoPost = new ProfilePageUser { ProfilePageID = page.ProfilePageID, UserId = user.Id, Date = DateTime.Now };

            _userRepository.SaveVisit(dtoPost);
        }
        public async Task<IActionResult> Edit()
        {
            return View(await GetUserViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserProfileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var claims = HttpContext.User;
                await _userRepository.SaveUser(claims, viewModel.user);
            }
        
            return View(await GetUserViewModel());
        }

        [HttpPost]
        public async Task<string> SaveFile(FileUpload objFile)
        {
            try
            {
                var claims = HttpContext.User;
                var user = await GetUser();

                objFile.CheckImage();

                using var stream = new MemoryStream();
                objFile.file.CopyTo(stream);
                var fileBytes = stream.ToArray();
                user.Image = fileBytes;
                await _userRepository.SaveImage(claims, user);

                return "Saved";
            }
            catch (WrongFileExtensionException e)
            {

                return e.ErrorMessage();

            }
        }
        [HttpPost]
        public async Task<string> SaveInterest(string dto)
        {
            var claims = HttpContext.User;
            var interest = _interestRepository.GetInterest(dto);
            try
            {
                await _userRepository.SaveInterest(claims, interest);
            }
            catch (ArgumentNullException)
            {
                return "Failed";
            }

            return "Saved";
        }
        public async Task DeleteInterest(string dto)
        {
            var claims = HttpContext.User;
            var interest = _interestRepository.GetInterest(dto);
            await _userRepository.DeleteInterest(claims, interest);

        }
        [HttpGet]
        public async Task<JsonResult> GetProfileImage()
        {
            var user = await GetUser();
            if (user.Image != null)
            {
                user.Image = GetImage(Convert.ToBase64String(user.Image));
            }
            return Json(user.Image);
        }
        [HttpGet]
        public JsonResult GetVisitorProfileImage(string id)
        {
            var user = _userRepository.GetUserByID(id);
            if (user.Image != null)
            {
                user.Image = GetImage(Convert.ToBase64String(user.Image));
            }
            return Json(user.Image);
        }
        private byte[] GetImage(string sBase64String)
        {
            byte[] bytes = null;
            if (!string.IsNullOrEmpty(sBase64String))
            {
                bytes = Convert.FromBase64String(sBase64String);
            }
            return bytes;
        }
        [HttpPost]
        public string ChangeCategory(string applicationID, string FriendID, string newCategory)
        {
            _friendRepository.SaveFriendshipCategory(applicationID, FriendID, newCategory);

            return "Changed";
        }
        [HttpPost]
        public async Task<JsonResult> QuitUserAccount()
        {
            var claims = HttpContext.User;
            await _userRepository.DeleteAccount(claims);
            await _signInManager.SignOutAsync();
            return Json("/ShowcaseUsers/Index");
        }

        [HttpPost]
        public async Task HideAccount()
        {
            var claims = HttpContext.User;
            await _userRepository.HideAccount(claims);

        }

        [HttpGet]
        public IActionResult UpdateMessageBoard(string id)
        {
            var user = _userRepository.GetUserByID(id);
            return PartialView("_MessageBoard", user);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfileMessageBoard()
        {
            var user = await GetUserViewModel();
            return PartialView("_ProfileMessageBoard", user);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateFriendCategory()
        {
            var user = await GetUserViewModel();
            return PartialView("_FriendList", user);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateInterestList()
        {
            var user = await GetUserViewModel();
            return PartialView("_InterestList", user);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateHideShowInfo()
        {
            var user = await GetUser();
            return PartialView("_HideShowAccount", user);
        }


    }
}
