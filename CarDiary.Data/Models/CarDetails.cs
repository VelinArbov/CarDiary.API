namespace CarDiary.Data.Models
{
    public class CarDetails
    {
        public Guid Id { get; set; }
        public required string Number { get; set; }
        public string? Description { get; set; }
    }
}
