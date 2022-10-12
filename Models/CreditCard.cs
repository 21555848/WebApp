namespace WebApp.Models
{
    public class CreditCard
    {
        public int Id { get; set; }
        public string? CardHolder { get; set; }
        public string? CreditCardNo { get; set; }
        public DateTime ? ExpiryDate { get; set; }
        public int ? CVV { get; set; }

        //Profile navigation property
        public int PatientProfileId { get; set; }
        public PatientProfile PatientProfile { get; set; }
        
    }
}