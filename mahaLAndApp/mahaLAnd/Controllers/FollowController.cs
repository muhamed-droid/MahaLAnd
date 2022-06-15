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

        // GET: Follow
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Follow.Include(f => f.Follower).Include(f => f.Following);
            return View(await applicationDbContext.ToListAsync());
        }

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
            /*foreach (var f in f1)
            {
                var user1 = _context.Users.First(u => u.Id.Equals(f.FollowerId));
                var profile1 = _context.Profile.First(p => p.UserId.Equals(user1.Id));
                followers.Add(new Tuple<User, Profile>(user1, profile1));
            } 
            followers.Reverse();*/
            for (int i = f1.Count - 1; i >= 0; i--)
            {
                var user1 = _context.Users.First(u => u.Id.Equals(f1[i].FollowerId));
                var profile1 = _context.Profile.First(p => p.UserId.Equals(user1.Id));
                followers.Add(new Tuple<User, Profile>(user1, profile1));
            }

            var loggedUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedProfile = _context.Profile.First(p => p.UserId.Equals(loggedUser.Id));
            //return View(profile);
            return View(new MyModel { LoggedUser = loggedUser, LoggedProfile = loggedProfile, Follow = followers });
            //var applicationDbContext = _context.Follow.Include(f => f.Follower).Include(f => f.Following);
            //return View(await applicationDbContext.ToListAsync());
        }

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
            /*foreach (var f in f2)
            {
                var user2 = _context.Users.First(u => u.Id.Equals(f.FollowingId));
                var profile2 = _context.Profile.First(p => p.UserId.Equals(user2.Id));
                following.Add(new Tuple<User, Profile>(user2, profile2));
            }
            following.Reverse();*/
            for (int i = f2.Count - 1; i >= 0; i--)
            {
                var user1 = _context.Users.First(u => u.Id.Equals(f2[i].FollowingId));
                var profile1 = _context.Profile.First(p => p.UserId.Equals(user1.Id));
                following.Add(new Tuple<User, Profile>(user1, profile1));
            }

            var loggedUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedProfile = _context.Profile.First(p => p.UserId.Equals(loggedUser.Id));
            //return View(profile);
            return View(new MyModel { LoggedUser = loggedUser, LoggedProfile = loggedProfile, Follow = following });
            //var applicationDbContext = _context.Follow.Include(f => f.Follower).Include(f => f.Following);
            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: Follow/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var follow = await _context.Follow
                .Include(f => f.Follower)
                .Include(f => f.Following)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (follow == null)
            {
                return NotFound();
            }

            return View(follow);
        }

        // GET: Follow/Create
        public IActionResult Create()
        {
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["FollowingId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
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
                    /*var notification = new Notification();
                    notification.Type = NotificationType.FOLLOW;
                    notification.UserId = UserId;
                    _context.Add(notification);
                    await _context.SaveChangesAsync();*/
                }
                else
                {
                    //var follow = new Follow();
                    var id = _context.Follow.First(f => f.FollowerId.Equals(FollowerId) && f.FollowingId.Equals(FollowingId)).Id;
                    var follow = await _context.Follow.FindAsync(id);
                    _context.Follow.Remove(follow);
                    await _context.SaveChangesAsync();
                    //follow.FollowerId = FollowerId;
                    //follow.FollowingId = FollowingId;
                    //_context.Follow.Remove(follow);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Details", "Profile", new { id = ProfileId } );
            }
            //ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "Id", follow.FollowerId);
            //ViewData["FollowingId"] = new SelectList(_context.Users, "Id", "Id", follow.FollowingId);
            return View();
        }

        // GET: Follow/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var follow = await _context.Follow.FindAsync(id);
            if (follow == null)
            {
                return NotFound();
            }
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "Id", follow.FollowerId);
            ViewData["FollowingId"] = new SelectList(_context.Users, "Id", "Id", follow.FollowingId);
            return View(follow);
        }

        // POST: Follow/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FollowerId,FollowingId")] Follow follow)
        {
            if (id != follow.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(follow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FollowExists(follow.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "Id", follow.FollowerId);
            ViewData["FollowingId"] = new SelectList(_context.Users, "Id", "Id", follow.FollowingId);
            return View(follow);
        }

        // GET: Follow/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var follow = await _context.Follow
                .Include(f => f.Follower)
                .Include(f => f.Following)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (follow == null)
            {
                return NotFound();
            }

            return View(follow);
        }

        // POST: Follow/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var follow = await _context.Follow.FindAsync(id);
            _context.Follow.Remove(follow);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool FollowExists(int id)
        {
            return _context.Follow.Any(e => e.Id == id);
        }
    }
}
