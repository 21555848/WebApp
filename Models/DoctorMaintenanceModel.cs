using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class DoctorMaintenanceModel
    {
        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
       
        [Display(Name = "Suite")]
        public int SuiteId { get; set; }

        [Required]
        [Display(Name ="Email Address")]
        public string EmailAddress { get; set; }
    }
}
