namespace WebApp.Models
{
    public class Work
    {
        public int Id { get; set; }
        public string? Company { get; set; }
        public string? Occupation { get; set; }
        public string? WorkPhone { get; set; }

        //Patient profile navigation property
        public int PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; }
    }
}
