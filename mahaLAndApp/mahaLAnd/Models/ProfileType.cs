using System.ComponentModel.DataAnnotations;

namespace mahaLAnd.Models
{
    public enum ProfileType
    {
        [Display(Name = "PERSONAL")]
        PERSONAL,
        [Display(Name = "PROFESSIONAL")]
        PROFESSIONAL
    }
}
