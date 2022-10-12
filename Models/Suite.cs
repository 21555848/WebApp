namespace WebApp.Models
{
    public class Suite
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //Doctor navigation property
       // public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

    }
}