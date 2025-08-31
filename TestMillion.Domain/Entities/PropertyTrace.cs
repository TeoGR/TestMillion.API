namespace TestMillion.Domain.Entities
{
    public class PropertyTrace
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public DateTime DateSale { get; set; }
        public decimal Price { get; set; }
    }
}
