using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mahaLAnd.Models
{
    public class Follow
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Follower")]
        public string FollowerId { get; set; }  //int
        public User Follower { get; set; }   //RegisteredUser

        [ForeignKey("Following")]
        public string FollowingId { get; set; }  //int
        public User Following { get; set; }   //RegisteredUser

        public Follow()
        {
        }
    }
}
