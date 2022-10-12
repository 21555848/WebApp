namespace WebApp.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //Appointments navigation property
        public List<Appointment> Appointments { get; set; }

        //Suite navigation property
        public int SuiteId { get; set; }
        public Suite Suite { get; set; }
    }
}