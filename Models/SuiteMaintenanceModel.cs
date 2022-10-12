using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class SuiteMaintenanceModel
    {
        [Required]
        [Display(Name ="Room Number")]
        public string SuiteName { get; set; }
    }
}
