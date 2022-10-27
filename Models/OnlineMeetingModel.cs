using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class OnlineMeetingModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Date { get; set; }
        public string Doctor { get; set; }
        [Required]
        [DataType(DataType.Url)]
        public string Link { get; set; }

    }
}
