using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mahaLAnd.Data;
using mahaLAnd.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace mahaLAnd.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<User> _userManager;

        public PostController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<User> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: Post/Details/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
           
            var loggedUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedProfile = _context.Profile.First(p => p.UserId.Equals(loggedUser.Id));
            var profile = _context.Profile.First(p => p.Id == post.ProfileId);
            var user = _context.Users.First(u => u.Id.Equals(profile.UserId));
            var likesNotification = _context.Notification.ToList().FindAll(n => n.PostId == post.Id && n.Type == NotificationType.LIKE);
            var commentsNotification = _context.Notification.ToList().FindAll(n => n.PostId == post.Id && n.Type == NotificationType.COMMENT);
            var comments = new List<Tuple<User, Profile, string>>();
            foreach (var comment in commentsNotification)
            {
                var user1 = _context.Users.First(u => u.Id.Equals(comment.UserId));
                var profile1 = _context.Profile.First(p => p.UserId.Equals(comment.UserId));
                comments.Add(new Tuple<User, Profile, string>(user1, profile1, comment.Comment));
            }
            return View(new MyModel { LoggedUser = loggedUser, User = user, LoggedProfile = loggedProfile, Profile = profile, Post = post, NumberOfLikes = likesNotification.Count, Comments = comments });
        }

        public async Task<IActionResult> Details1(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            var profile = _context.Profile.First(p => p.Id == post.ProfileId);
            var user = _context.Users.First(u => u.Id.Equals(profile.UserId));
            var likesNotification = _context.Notification.ToList().FindAll(n => n.PostId == post.Id && n.Type == NotificationType.LIKE);
            var commentsNotification = _context.Notification.ToList().FindAll(n => n.PostId == post.Id && n.Type == NotificationType.COMMENT);
            var comments = new List<Tuple<User, Profile, string>>();
            foreach (var comment in commentsNotification)
            {
                var user1 = _context.Users.First(u => u.Id.Equals(comment.UserId));
                var profile1 = _context.Profile.First(p => p.UserId.Equals(comment.UserId));
                comments.Add(new Tuple<User, Profile, string>(user1, profile1, comment.Comment));
            }
            return View(new MyModel { User = user, Profile = profile, Post = post, NumberOfLikes = likesNotification.Count, Comments = comments });
        }

        // GET: Post/Create
        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            ViewData["ProfileId"] = new SelectList(_context.Profile, "Id", "Id");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostFile,Description")] Post post)
        {
            if (ModelState.IsValid)
            {
                var user = (await _userManager.GetUserAsync(HttpContext.User));
                post.ProfileId = _context.Profile.First(p => p.UserId.Equals(user.Id)).Id;
                post.Date = DateTime.Now;
                string wwwRoothPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(post.PostFile.FileName);
                string extension = Path.GetExtension(post.PostFile.FileName);
                post.PostURL = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRoothPath + "/img/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await post.PostFile.CopyToAsync(fileStream);
                }
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Profile", new { id = post.ProfileId });
            }
            ViewData["ProfileId"] = new SelectList(_context.Profile, "Id", "Id", post.ProfileId);
            return View(post);
        }

        // GET: Post/Edit/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["ProfileId"] = new SelectList(_context.Profile, "Id", "Id", post.ProfileId);
            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    post.ProfileId = _context.Profile.First(p => p.UserId.Equals(user.Id)).Id;

                    _context.Attach(post);
                    _context.Entry(post).Property(x => x.Description).IsModified = true;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", post);
            }
            ViewData["ProfileId"] = new SelectList(_context.Profile, "Id", "Id", post.ProfileId);
            return View(post);
        }

        // GET: Post/Delete/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Profile", new { id = post.ProfileId });
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Statistics(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            var profile = _context.Profile.First(p => p.Id == post.ProfileId);
            var statistics = new Statistics();
            var likes = _context.Notification.ToList().FindAll(n => n.PostId == post.Id && n.Type == NotificationType.LIKE);
            statistics.NumberOfLikes = likes.Count;
            statistics.NumberOfMaleLikes = likes.FindAll(l => (_context.Users.First(u => u.Id.Equals(l.UserId))).Gender == Gender.MALE).Count;
            statistics.NumberOfFemaleLikes = likes.FindAll(l => (_context.Users.First(u => u.Id.Equals(l.UserId))).Gender == Gender.FEMALE).Count;
            statistics.NumberOfComments = _context.Notification.ToList().FindAll(n => n.PostId == post.Id && n.Type == NotificationType.COMMENT).Count;
            var user = _context.Users.First(u => u.Id.Equals(profile.UserId));
            return View(new MyModel { Profile = profile, Post = post, Statistics = statistics });
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Likes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            var loggedUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedProfile = _context.Profile.First(p => p.UserId.Equals(loggedUser.Id));
            var likesNotification = _context.Notification.ToList().FindAll(n => n.PostId == post.Id && n.Type == NotificationType.LIKE);
            var likes = new List<Tuple<User, Profile>>();
            foreach (var like in likesNotification)
            {
                var user1 = _context.Users.First(u => u.Id.Equals(like.UserId));
                var profile1 = _context.Profile.First(p => p.UserId.Equals(like.UserId));
                likes.Add(new Tuple<User, Profile>(user1, profile1));
            }
            return View(new MyModel { LoggedUser = loggedUser, LoggedProfile = loggedProfile, Likes = likes });
        }
    }
}
