namespace PurchaseApi.Web.Dto
{
    public class OrderItemDto
    {
        public string TourId { get; set; } = default!;
        public string TourName { get; set; } = default!;
        public decimal Price { get; set; }
    }
}
