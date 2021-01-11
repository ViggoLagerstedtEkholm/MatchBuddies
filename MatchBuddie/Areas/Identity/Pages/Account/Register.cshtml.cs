using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using DataLayer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using DataLayer.Data.Models;

namespace MatchBuddie.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly MatchBuddieContext _dbContext; 

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            MatchBuddieContext dbcontext 
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _dbContext = dbcontext; 
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Age")]
            [Range(18, 100, ErrorMessage = "You must be at least 18 years old but not older than 100.")]
            public int Age { get; set; }

            [Required]
            [Display(Name = "Sex")]
            public MatchPreference Preference { get; set; }

            [Required]
            [RegularExpression("(.*[a-zA-ZäåöüÄÅÖÜß]){3}", ErrorMessage ="Your city must atleast have three letters")]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [Display(Name = "Gender")]
            public GenderType Gender { get; set; }

            [Required]
            [RegularExpression("[a-zA-ZäåöüÄÅÖÜß]+", ErrorMessage = "Firstname can only contain letters")]
            [Display(Name = "Firstname")]
            public string Firstname { get; set; }

            [Required]
            [RegularExpression("[a-zA-ZäåöüÄÅÖÜß]+", ErrorMessage = "Lastname can only contain letters")]
            [Display(Name = "Lastname")]
            public string Lastname { get; set; }

            [Required]
            [EmailAddress(ErrorMessage = "Something went wrong try changing password or email")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        
            if (ModelState.IsValid)
            {

                var user = new User
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Firstname = Input.Firstname,
                    Lastname = Input.Lastname,
                    City = Input.City,
                    Gender = Input.Gender,
                    Preference = Input.Preference,
                    age = Input.Age
                };

                if (_dbContext.User.Where(x => x.Email == Input.Email).ToList().Count > 0)
                {
                    ModelState.AddModelError("EmailExists", "Invalid Email or Password.");
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                   await  _dbContext.ProfilePage.AddAsync(new ProfilePage { Owner = user.Id }); 
                   await _dbContext.SaveChangesAsync(); 
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
