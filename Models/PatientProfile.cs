using System.ComponentModel.DataAnnotations;
using WebApp.Areas.Identity.Data;

namespace WebApp.Models
{
    public class PatientProfile
    {
        public int Id { get; set; }
       // public string FirstName { get; set; }
       // public string LastName { get; set; }
        public string CellNo { get; set; }
        public string? AlternateCell { get; set; }
        public string EmailAddress { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DoB { get; set; }
        public Gender? Gender { get; set; }

        public int PatientAddressId { get; set; }
        public PatientAddress? PatientAddress { get; set; }

        public int WorkId { get; set; }
        public Work? Work { get; set; }

        //Medical Aid information
        public int MedicalAidId { get; set; }
        public MedicalAid? MedicalAid  { get; set; }

        //Appointments navigation property
        public List<Appointment>? Appointments { get; set; }

        //User navigation property
        //public int UserId { get; set; }
        //public User User { get; set; }
        public string WebAppUserId { get; set; }
        public WebAppUser WebAppUser { get; set; }

    }

    public enum Gender
    {
        Male,
        Female
    }
}