using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mahaLAnd.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }   //int
        public User User { get; set; }    //RegisteredUser

        [ForeignKey("Post")]
        public int PostId { get; set; }   
        public Post Post { get; set; }

        public NotificationType Type { get; set; }

        public string Comment { get; set; }

        public Notification()
        {
        }
        /*public Notification(int id, RegisteredUser user, Post post, NotificationType type, string comment)
        {
            Id = id;
            User = user;
            Post = post;
            Type = type;
            Comment = comment;
        }
        public Notification(RegisteredUser user, Post post, NotificationType type)
        {
            User = user;
            Post = post;
            Type = type;
        }*/
    }
}
