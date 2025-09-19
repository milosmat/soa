namespace PurchaseApi.Web.Dto
{
    public class TourDto
    {
        public string Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Difficulty { get; set; } = default!;
        public decimal Price { get; set; }
        public double LengthKm { get; set; }
        public List<TourPointDto> Points { get; set; } = new();
    }
}
