using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mahaLAnd.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }   //int
        public User User { get; set; }  //RegisteredUser

        public Request()
        {
        }
    }
}
