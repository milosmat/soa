namespace PurchaseApi.Web.Dto
{
    public class TourPointDto
    {
        public string Id { get; set; } = default!;
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Name { get; set; } = default!;
    }
}
