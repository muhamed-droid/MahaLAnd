using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using mahaLAnd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using mahaLAnd.Controllers;
using mahaLAnd.Data;

namespace mahaLAnd.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [RegularExpression(@"[A-Z]+[a-z]*", ErrorMessage = "The Name field is not a valid name.")]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [RegularExpression(@"[A-Z]+[a-z]*", ErrorMessage = "The Surname field is not a valid surname.")]
            [Display(Name = "Surname")]
            public string Surname { get; set; }

            [Required]
            [RegularExpression(@"[0-9|a-z|A-Z|.|_|-]*", ErrorMessage = "The Username field is not a valid username.")]
            [Display(Name = "UserName")]
            public string UserName { get; set; }

            [Required]
            [Range(13, 100, ErrorMessage = "The Age must be between 13 and 100.")]
            [Display(Name = "Age")]
            public int Age { get; set; }

            [Required]
            [Display(Name = "Gender")]
            [EnumDataType(typeof(Gender))]
            public Gender Gender { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
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
                var user = new User { Email = Input.Email, Name = Input.Name, Surname = Input.Surname, UserName = Input.UserName, Age = Input.Age, Gender = Input.Gender  };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    _context.Add(new Profile { User = user, ProfilePhoto = "user.png" });
                    await _context.SaveChangesAsync();

                    /*MyModel model = new MyModel
                    {
                        User = user,
                        Profile = _context.Profile.First(p => p.UserId.Equals(user.Id))
                    };
                    TempData.Put("myModel", model);
                    return RedirectToAction("Index1", "Profile");*/
                    return RedirectToPage("Login");
                    //return RedirectToAction("Index", "Profile", new { id = _context.Profile.First(p => p.UserId.Equals(user.Id)).Id });

                    //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", profile.UserId);
                    //return View(profile);

                    /*await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }*/
                    //return await new ProfileController(_context).Create(new Profile{ User = user });
                    //return RedirectToAction("Create", "Profile", new { User = user });
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
