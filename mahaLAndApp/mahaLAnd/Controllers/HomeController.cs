using mahaLAnd.Data;
using mahaLAnd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace mahaLAnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        /*public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }*/

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Search(string SearchedUser)
        {
            if (SearchedUser == null)
            {
                SearchedUser = "";
            }
            List<User> us = _context.Users.ToList().FindAll(u => u.UserName.ToLower().Equals(SearchedUser.ToLower()) || SearchedUser.ToLower().Contains(u.Name.ToLower()) || SearchedUser.ToLower().Contains(u.Surname.ToLower()));
            List<Tuple<User, Profile>> users = new List<Tuple<User, Profile>>();
            foreach (var u in us)
            {
                users.Add(new Tuple<User, Profile>(u, _context.Profile.First(p => p.UserId.Equals(u.Id))));
            }
            var loggedUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedProfile = _context.Profile.First(p => p.UserId.Equals(loggedUser.Id));
            return View(new MyModel { LoggedUser = loggedUser, LoggedProfile = loggedProfile, PotentionalUsers = users }); ;
        }


        public async Task<IActionResult> Search1(string SearchedUser)
        {
            if (SearchedUser == null)
            {
                SearchedUser = "";
            }
            List<User> us = _context.Users.ToList().FindAll(u => u.UserName.ToLower().Equals(SearchedUser.ToLower()) || SearchedUser.ToLower().Contains(u.Name.ToLower()) || SearchedUser.ToLower().Contains(u.Surname.ToLower()));
            List<Tuple<User, Profile>> users = new List<Tuple<User, Profile>>();
            foreach (var u in us)
            {
                users.Add(new Tuple<User, Profile>(u, _context.Profile.First(p => p.UserId.Equals(u.Id))));
            }
            return View(new MyModel { PotentionalUsers = users }); ;
        }
    }
}
