using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using mahaLAnd.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using mahaLAnd.Controllers;
using mahaLAnd.Data;

namespace mahaLAnd.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context;

        public LoginModel(SignInManager<User> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Profile/Index");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, false, /*Input.RememberMe, */lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = _context.Users.First(x => x.UserName.Equals(Input.UserName));
                    _logger.LogInformation("User logged in.");
                    //returnUrl ??= Url.Content("~/Profile/Index/" + _context.Profile.First(u => u.UserId == _context.Users.First(x => x.UserName == Input.UserName).Id).Id);
                    //return await new ProfileController(_context).Index(_context.Profile.First(u => u.UserId.Equals(_context.Users.First(x => x.UserName.Equals(Input.UserName)).Id)).Id);
                    //return LocalRedirect(returnUrl);
                    //TempData["myModel"] = model;

                    /*MyModel model = new MyModel();
                    model.User = user;
                    model.Profile = _context.Profile.First(p => p.UserId.Equals(user.Id));*/
                    /*TempData.Put("myModel", model);
                    return RedirectToAction("Index1", "Profile");*/

                    return RedirectToAction("Index", "Profile", new { id = _context.Profile.First(p => p.UserId.Equals(user.Id)).Id } );
                    //return RedirectToAction("Index", "Profile", new { model = new MyModel { User = user, Profile = _context.Profile.First(p => p.UserId.Equals(user.Id)) } });
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl/*, RememberMe = Input.RememberMe*/ });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
