namespace WebApp.Models
{
    public class PatientAddress
    {
        public int Id { get; set; }
        public string? StreetAddress { get; set; }
        public string? Address2 { get; set; }
        public string? Suburb { get; set; }
        public string? Province { get; set; }

        //Patient profile navigation property
        public int PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; }

    }
}
