namespace EatEat.Services.EmailAPI.Models
{
    public class EmailLog
    {
        public int ID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? EmailSent { get; set; }
    }
}
