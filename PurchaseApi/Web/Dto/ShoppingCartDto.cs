namespace PurchaseApi.Web.Dto
{
    public class ShoppingCartDto
    {
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}
