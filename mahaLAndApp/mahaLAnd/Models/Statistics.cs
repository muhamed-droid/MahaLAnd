using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mahaLAnd.Models
{
    public class Statistics
    {
        [Key]
        public int Id { get; set; }

        public int NumberOfLikes { get; set; }
        public int NumberOfComments { get; set; }
        public int NumberOfMaleLikes { get; set; }
        public int NumberOfFemaleLikes { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }

        /*public int NumberOfLikes { get => numberOfLikes; set => numberOfLikes = Post.Likes.Count; }
        public int NumberOfComments { get => numberOfComments; set => numberOfComments = Post.Comments.Count; }
        public int NumberOfMaleLikes { get => numberOfMaleLikes; set => numberOfMaleLikes = Post.Likes.RemoveAll(user => user.Gender == Gender.FEMALE); }
        public int NumberOfFemaleLikes { get => numberOfFemaleLikes; set => numberOfFemaleLikes = Post.Likes.RemoveAll(user => user.Gender == Gender.MALE); }
        */
        public Statistics()
        {
        }
        //public Statistics(Post post) => Post = post ?? throw new ArgumentNullException(nameof(post));


    }
}
