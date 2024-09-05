namespace MeterReading.Database.Models
{
    public class Account(string customerName)
    {
        public int AccountId { get; set; }
        public string CustomerName { get; init; } = customerName;
    }

    public class Reading()
    {
        
        public int MeterValue { get; init; } 
        public int AccountId { get; init; } 
        public DateTime MeterReadDate { get; init; } 
    }
}