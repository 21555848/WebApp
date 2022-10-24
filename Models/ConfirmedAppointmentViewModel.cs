using System.ComponentModel.DataAnnotations;
using WebApp.Areas.Identity.Data;

namespace WebApp.Models
{
    public class ConfirmedAppointmentViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNo { get; set; }
        public string? AlternateCell { get; set; }
        public string EmailAddress { get; set; }
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}")]
        public DateOnly Date { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{HH:mm}")]
        public TimeOnly Time { get; set; }
        public bool Approved { get; set; } = false;
        public int PIN { get; set; }

        //Doctor navigation property
        public WebAppUser Doctor { get; set; }

        public AppointmentType Type { get; set; }
    }
}
