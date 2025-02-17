namespace WebApplication1.Models
{
    public class Recommendation
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int PatientId { get; set; } 
    }
}
