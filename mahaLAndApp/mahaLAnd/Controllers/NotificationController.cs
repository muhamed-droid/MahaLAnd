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
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public NotificationController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Notification
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
        {
            var loggedUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedProfile = _context.Profile.First(p => p.UserId.Equals(loggedUser.Id));
            var posts = _context.Post.ToList().FindAll(p => p.ProfileId == loggedProfile.Id);
            var postsId = new List<int>();
            foreach (var post in posts)
            {
                postsId.Add(post.Id);
            }
            var notifs = _context.Notification.ToList().FindAll(n => postsId.Contains(n.PostId));
            var notifications = new List<Tuple<Notification, User, Profile>>();
            for (int i = notifs.Count - 1; i >= 0; i--)
            {
                var user = _context.Users.First(u => u.Id.Equals(notifs[i].UserId));
                var profile = _context.Profile.First(p => p.UserId.Equals(notifs[i].UserId));
                if (!user.Id.Equals(loggedUser.Id))
                    notifications.Add(new Tuple<Notification, User, Profile>(notifs[i], user, profile));
            }
            return View(new MyModel { LoggedUser = loggedUser, LoggedProfile = loggedProfile, Notifications = notifications } );
        }

        // POST: Notification/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(string Comment, int PostId, string UserId)
        {
            var notification = new Notification();
            notification.PostId = PostId;
            notification.Comment = Comment;
            notification.Type = NotificationType.COMMENT;
            notification.UserId = UserId;

            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Post", new { id = notification.PostId } );
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLike(int PostId, string UserId)
        {
            var notification = new Notification();
            notification.PostId = PostId;
            notification.Type = NotificationType.LIKE;
            notification.UserId = UserId;

            if (ModelState.IsValid)
            {
                var exist = new List<Notification>();
                exist = _context.Notification.ToList().FindAll(n => n.UserId.Equals(UserId) && n.Type == NotificationType.LIKE && n.PostId.Equals(PostId));
                if(exist.Count == 0)
                {
                    _context.Add(notification);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Details", "Post", new { id = PostId });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLike1(int PostId, string UserId)
        {
            var notification = new Notification();
            notification.PostId = PostId;
            notification.Type = NotificationType.LIKE;
            notification.UserId = UserId;

            if (ModelState.IsValid)
            {
                var loggedProfile = _context.Profile.First(p => p.UserId.Equals(UserId));
                var exist = new List<Notification>();
                exist = _context.Notification.ToList().FindAll(n => n.UserId.Equals(UserId) && n.Type == NotificationType.LIKE && n.PostId.Equals(PostId));
                if (exist.Count == 0)
                {
                    _context.Add(notification);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Profile", new { id = loggedProfile.Id });
            }
            return View();
        }

        // POST: Notification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            _context.Notification.Remove(notification); 
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notification.Any(e => e.Id == id);
        }
    }
}
