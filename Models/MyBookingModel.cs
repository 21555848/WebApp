using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class MyBookingModel
    {
        [Required]
        [Display(Name ="Booking Reference")]
        public int Reference { get; set; }
        [Required]
        [Display(Name ="PIN")]
        public int PIN { get; set; }
    }
}
