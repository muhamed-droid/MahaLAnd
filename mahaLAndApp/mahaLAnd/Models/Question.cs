using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mahaLAnd.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }   //int
        public User User { get; set; }   //RegisteredUser

        //[Required(ErrorMessage = "Please write a question")]
        [DisplayName("Question")]
        public string Text { get; set; }

        public string Answer { get; set; }

        public Question()
        {
        }
    }
}
