using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mahaLAnd.Data;
using mahaLAnd.Models;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace mahaLAnd.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<User> _userManager;

        public ProfileController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<User> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: Profile
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            { 
                return NotFound();
            }

            var profile = await _context.Profile
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (profile == null)
            {
                return NotFound();
            }

            var user = _context.Users.First(u => u.Id.Equals(profile.UserId));

            List<Tuple<User, Profile, Post, int>> following = new List<Tuple<User, Profile, Post, int>>();
            List<Follow> f2 = _context.Follow.ToList().FindAll(f => f.FollowerId.Equals(user.Id));
            List<int> profiliId = new List<int>();
            foreach (var f in f2)
            {
                var user2 = _context.Users.First(u => u.Id.Equals(f.FollowingId));
                var profile2 = _context.Profile.First(p => p.UserId.Equals(user2.Id));
                profiliId.Add(profile2.Id);
            }
            var posts = _context.Post.ToList().FindAll(p => profiliId.Contains(p.ProfileId));
            /*foreach (var post in posts)
            {
                var profile2 = _context.Profile.First(p => p.Id == post.ProfileId);
                var user2 = _context.Users.First(u => u.Id.Equals(profile2.UserId));
                var likesNotification = _context.Notification.ToList().FindAll(n => n.PostId == post.Id && n.Type == NotificationType.LIKE).Count;
                following.Add(new Tuple<User, Profile, Post, int>(user2, profile2, post, likesNotification));
            }
            following.Reverse();*/
            for (int i = posts.Count - 1; i >= 0; i--)
            {
                var profile2 = _context.Profile.First(p => p.Id == posts[i].ProfileId);
                var user2 = _context.Users.First(u => u.Id.Equals(profile2.UserId));
                var likesNotification = _context.Notification.ToList().FindAll(n => n.PostId == posts[i].Id && n.Type == NotificationType.LIKE).Count;
                following.Add(new Tuple<User, Profile, Post, int>(user2, profile2, posts[i], likesNotification));
            }

            //return View(profile);
            return View(new MyModel { User = user, Profile = profile, Feed = following });
            //var applicationDbContext = _context.Profile.Include(p => p.User);
            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: Profile
        /*public async Task<IActionResult> Index1()
        {
            var model = TempData.Get<MyModel>("myModel");

            if (model.User == null || model.Profile == null)
            {
                return NotFound();
            }
            /*var profile = await _context.Profile
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == model.Profile.Id);
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id.Equals(model.User.Id));*/

            /*if (profile == null)
            {
                return NotFound();
            }

            //return View(model);
             return RedirectToAction("Index", model.Profile.Id);
            //return View(tuple);
            //var applicationDbContext = _context.Profile.Include(p => p.User);
            //return View(await applicationDbContext.ToListAsync());
        }*/

        /*public async Task<IActionResult> Index2(MyModel model)
        {
            if (model.User == null || model.Profile == null)
            {
                return NotFound();
            }
            /*var profile = await _context.Profile
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == model.Profile.Id);
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id.Equals(model.User.Id));*/

            /*if (profile == null)
            {
                return NotFound();
            }
            return View(model);
            //return View(tuple);
            //var applicationDbContext = _context.Profile.Include(p => p.User);
            //return View(await applicationDbContext.ToListAsync());
        }*/

        // GET: Profile/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profile
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            var user = _context.Users.First(u => u.Id.Equals(profile.UserId));
            List<Post> posts = _context.Post.ToList().FindAll(p => p.ProfileId == profile.Id);
            posts.Reverse();
            List<Follow> f1 = _context.Follow.ToList().FindAll(f => f.FollowingId.Equals(user.Id));
            List<User> followers = new List<User>();
            /*foreach (var f in f1)
            {
                followers.Add(_context.Users.First(u => u.Id.Equals(f.FollowerId)));
            }
            followers.Reverse();*/
            for (int i = f1.Count - 1; i >= 0; i--)
            {
                followers.Add(_context.Users.First(u => u.Id.Equals(f1[i].FollowerId)));
            }
            List<Follow> f2 = _context.Follow.ToList().FindAll(f => f.FollowerId.Equals(user.Id));
            List<User> following = new List<User>();
            /*foreach (var f in f2)
            {
                following.Add(_context.Users.First(u => u.Id.Equals(f.FollowingId)));
            }
            following.Reverse();*/
            for (int i = f2.Count - 1; i >= 0; i--)
            {
                following.Add(_context.Users.First(u => u.Id.Equals(f2[i].FollowingId)));
            }
            //return View(profile);
            var loggedUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedProfile = _context.Profile.First(p => p.UserId.Equals(loggedUser.Id));
            return View(new MyModel { LoggedUser = loggedUser, User = user, LoggedProfile = loggedProfile, Profile = profile, Posts = posts, Followers = followers, Following = following });
        }

        public async Task<IActionResult> Details1(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profile
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            var user = _context.Users.First(u => u.Id.Equals(profile.UserId));
            List<Post> posts = _context.Post.ToList().FindAll(p => p.ProfileId == profile.Id);
            posts.Reverse();
            List<Follow> f1 = _context.Follow.ToList().FindAll(f => f.FollowingId.Equals(user.Id));
            List<User> followers = new List<User>();
            /*foreach (var f in f1)
            {
                followers.Add(f.Follower);
            }
            followers.Reverse();*/
            for (int i = f1.Count - 1; i >= 0; i--)
            {
                followers.Add(_context.Users.First(u => u.Id.Equals(f1[i].FollowerId)));
            }
            List<Follow> f2 = _context.Follow.ToList().FindAll(f => f.FollowerId.Equals(user.Id));
            List<User> following = new List<User>();
            /*foreach (var f in f2)
            {
                following.Add(f.Following);
            }
            following.Reverse();*/
            for (int i = f2.Count - 1; i >= 0; i--)
            {
                following.Add(_context.Users.First(u => u.Id.Equals(f2[i].FollowingId)));
            }
            //return View(profile);
            return View(new MyModel { User = user, Profile = profile, Posts = posts, Followers = followers, Following = following });
        }

        // GET: Profile/Details/5
        /*public async Task<IActionResult> Details1()
        {
            var model = TempData.Get<MyModel>("myModel");

            if (model.User == null || model.Profile == null)
            {
                return NotFound();
            }

            /*var profile = await _context.Profile
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == model.Profile.Id);
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id.Equals(model.User.Id));
            if (profile == null || user == null)
            {
                return NotFound();
            }

            return View(model);
        }*/

        /*public async Task<IActionResult> Details2(MyModel model)
        {
            //var model = TempData.Get<MyModel>("myModel");

            if (model.User == null || model.Profile == null)
            {
                return NotFound();
            }

            /*var profile = await _context.Profile
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == model.Profile.Id);
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id.Equals(model.User.Id));
            if (profile == null || user == null)
            {
                return NotFound();
            }

            return View(model);
        }*/

        // GET: Profile/Create
        /*public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }*/

        // POST: Profile/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ProfilePhoto,Description,ProfileType")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                profile.UserId = user.Id;

                _context.Add(profile);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", profile.UserId);
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", profile.UserId);
            return View(profile);
            //return RedirectToPage("/Register");
        }

        // GET: Profile/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profile.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", profile.UserId);
            return View(profile);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ProfilePhotoFile,Description,ProfileType")] Profile profile)
        {
            if (id != profile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    profile.UserId = user.Id;

                    if (profile.ProfilePhotoFile != null)
                    {
                        string wwwRoothPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(profile.ProfilePhotoFile.FileName);
                        string extension = Path.GetExtension(profile.ProfilePhotoFile.FileName);
                        profile.ProfilePhoto = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        //profile.ProfilePhoto = fileName;
                        string path = Path.Combine(wwwRoothPath + "/img/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await profile.ProfilePhotoFile.CopyToAsync(fileStream);
                        }
                    }
                    //var prof = new Profile() { Id = id, ProfilePhoto = profile.ProfilePhoto, ProfileType = profile.ProfileType };
                    //_context.Profile.Attach(prof);
                    //_context.Entry(prof).Property(x => x.Description).IsModified = true;
                    //if(profile.ProfilePhoto != null)
                    //    _context.Entry(prof).Property(x => x.ProfilePhoto).IsModified = true;
                    //_context.Entry(prof).Property(x => x.ProfileType).IsModified = true;
                    else
                        profile.ProfilePhoto = "user.png";
                    _context.Update(profile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfileExists(profile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", profile);
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", profile.UserId);
            return View(profile);
        }

        // GET: Profile/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profile
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        // POST: Profile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profile = await _context.Profile.FindAsync(id);
            _context.Profile.Remove(profile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfileExists(int id)
        {
            return _context.Profile.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Contact(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", profile.UserId);
            return View(user);
        }
    }
}
