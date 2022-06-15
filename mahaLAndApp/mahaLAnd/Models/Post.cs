using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mahaLAnd.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string PostURL { get; set; }

        public string Description { get; set; }

        [ForeignKey("Profile")]
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        //public List<RegisteredUser> Likes { get; set; }
        //public List<RegisteredUser> Comments { get; set; }
        //public Statistics Statistics { get; set; }

        [NotMapped]
        public IFormFile PostFile { get; set; }

        public Post()
        {
        }
        /*public Post(int id, string postString)
        {
            Id = id;
            PostString = postString;
            Likes = new List<RegisteredUser>();
            Comments = new List<RegisteredUser>();
            //Statistics = new Statistics();
        }*/

        /*public void AddLike(RegisteredUser user) => Likes.Add(user);
        public void RemoveLike(RegisteredUser user) => Likes.Remove(user);
        public void AddComment(RegisteredUser user) => Comments.Add(user);
        public void RemoveComment(RegisteredUser user) => Comments.Remove(user);*/
    }
}
