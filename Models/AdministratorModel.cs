using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class AdministratorModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name ="Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name ="Email")]
        public string EmailAddress { get; set; }
    }
}