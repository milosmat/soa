namespace PurchaseApi.Web.Dto
{
    public class CheckoutResponse
    {
        public List<string> PurchasedTourIds { get; set; } = new();
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    }
}
