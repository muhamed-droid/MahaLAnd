using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mahaLAnd.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }   //int
        public User User { get; set; }   //RegisteredUser

        //public List<Post> Posts { get; set; }
        public string ProfilePhoto { get; set; }
        public string Description { get; set; }

        public ProfileType ProfileType { get; set; }
        //public List<RegisteredUser> Followers { get; set; } 
        //public List<RegisteredUser> Following { get; set; }

        [NotMapped]
        public IFormFile ProfilePhotoFile { get; set; }

        public Profile()
        {
        }
        /*public Profile(int id)
        {
            Id = id;
            Posts = new List<Post>();
            ProfilePhoto = string.Empty;
            Description = string.Empty;
            ProfileType = ProfileType.PERSONAL;
            Followers = new List<RegisteredUser>();
            Following = new List<RegisteredUser>();
        }*/

        public Profile(User user)
        {
            User = user;
            ProfileType = ProfileType.PERSONAL;
        }


        /*public void AddFollower(RegisteredUser user) => Followers.Add(user);
        public void RemoveFollower(RegisteredUser user) => Followers.Remove(user);
        public void AddFollowing(RegisteredUser user) => Following.Add(user);
        public void RemoveFollowing(RegisteredUser user) => Following.Remove(user);*/
    }
}
