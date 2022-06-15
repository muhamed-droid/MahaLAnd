using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mahaLAnd.Data;
using mahaLAnd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace mahaLAnd.Controllers
{
    [Authorize]
    public class FollowController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public FollowController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Followers(int? id)
        {
            var profile = await _context.Profile
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            var user = _context.Users.First(u => u.Id.Equals(profile.UserId));

            List<Tuple<User, Profile>> followers = new List<Tuple<User, Profile>>();
            List<Follow> f1 = _context.Follow.ToList().FindAll(f => f.FollowingId.Equals(user.Id));
            for (int i = f1.Count - 1; i >= 0; i--)
            {
                var user1 = _context.Users.First(u => u.Id.Equals(f1[i].FollowerId));
                var profile1 = _context.Profile.First(p => p.UserId.Equals(user1.Id));
                followers.Add(new Tuple<User, Profile>(user1, profile1));
            }

            var loggedUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedProfile = _context.Profile.First(p => p.UserId.Equals(loggedUser.Id));
            return View(new MyModel { LoggedUser = loggedUser, LoggedProfile = loggedProfile, Follow = followers });
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Following(int? id)
        {
            var profile = await _context.Profile
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            var user = _context.Users.First(u => u.Id.Equals(profile.UserId));

            List<Tuple<User, Profile>> following = new List<Tuple<User, Profile>>();
            List<Follow> f2 = _context.Follow.ToList().FindAll(f => f.FollowerId.Equals(user.Id));
            for (int i = f2.Count - 1; i >= 0; i--)
            {
                var user1 = _context.Users.First(u => u.Id.Equals(f2[i].FollowingId));
                var profile1 = _context.Profile.First(p => p.UserId.Equals(user1.Id));
                following.Add(new Tuple<User, Profile>(user1, profile1));
            }
            var loggedUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedProfile = _context.Profile.First(p => p.UserId.Equals(loggedUser.Id));
            return View(new MyModel { LoggedUser = loggedUser, LoggedProfile = loggedProfile, Follow = following });
        }

        // POST: Follow/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string FollowerId, string FollowingId, int Follow, int ProfileId)
        {
            if (ModelState.IsValid)
            {
                if(Follow == 1)
                {
                    var follow = new Follow();
                    follow.FollowerId = FollowerId;
                    follow.FollowingId = FollowingId;
                    _context.Add(follow);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var id = _context.Follow.First(f => f.FollowerId.Equals(FollowerId) && f.FollowingId.Equals(FollowingId)).Id;
                    var follow = await _context.Follow.FindAsync(id);
                    _context.Follow.Remove(follow);
                    await _context.SaveChangesAsync();
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Details", "Profile", new { id = ProfileId } );
            }
            return View();
        }

        private bool FollowExists(int id)
        {
            return _context.Follow.Any(e => e.Id == id);
        }
    }
}
