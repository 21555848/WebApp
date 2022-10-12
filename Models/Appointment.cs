using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNo { get; set; }
        public string ?AlternateCell { get; set; }
        public string EmailAddress { get; set; }
        [DisplayFormat(DataFormatString ="{0: dd/MM/yyyy}")]
        public DateOnly Date { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{HH:mm}")]
        public TimeOnly Time { get; set; }
        public bool Approved { get; set; } = false;
        public int PIN { get; set; }

        //Patient navigation property
        public int ?PatientId { get; set; } //Change to PatientProfileId
        public PatientProfile ?PatientProfile { get; set; }

        //Doctor navigation property
        public int ?DoctorId { get; set; }
        public Doctor ?Doctor { get; set; }

        public AppoitnmentType Type { get; set; }


    }

    public enum AppoitnmentType
    {
        ClinicVisit,
        HomeCall,
        OnlineConsultation
    }
}
