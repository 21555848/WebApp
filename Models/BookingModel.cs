using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class BookingModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Cellphone Number")]
        public string CellNo { get; set; }

        [Display(Name = "Alternate Contact Number")]
        [Phone]
        public string? AlternateCell { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy")]
        [Display(Name ="Appointment Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }


        [DisplayFormat(DataFormatString = "{0: HH:mm}")]
        [Display(Name ="Appointment Time")]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        [Display(Name ="Street Address")]
        public string? StreetAddress { get; set; }
        [Display(Name ="Unit Number & Complex")]
        public string? Address2 { get; set; }
        public string? Suburb { get; set; }
        public string? Province { get; set; }
        [Display(Name ="Postal Code")]
        public string? PostalCode { get; set; }
        public bool UseDifferentAddress { get; set; }
        [Display(Name = "Street Address")]
        public string? altStreetAddress { get; set; }
        [Display(Name = "Unit Number & Complex")]
        public string? altAddress2 { get; set; }
        [Display(Name ="Suburb")]
        public string? altSuburb { get; set; }
        [Display(Name ="Province")]
        public string? altProvince { get; set; }
        [Display(Name = "Postal Code")]
        public string? altPostalCode { get; set; }

    }
}
