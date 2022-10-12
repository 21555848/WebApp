namespace WebApp.Models
{
    public class MedicalAid
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Number { get; set; }

        //Profile nagivation property
        public int PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; }

    }
}