using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PurchaseApi.Store;
using PurchaseApi.Store.Model;
using PurchaseApi.Web.Dto;
using PurchaseApi.Web.Auth;
using PurchaseApi.Saga;
using CPSaga = Contracts.Saga.CreatePurchase;

namespace PurchaseApi.Web.Controllers
{
    [ApiController]
    [Route("cart")]
    [Authorize]
    public class ShoppingCartController : ControllerBase
    {
        private readonly AppDb _db;
        public ShoppingCartController(AppDb db) => _db = db;

        // GET /cart → vraća trenutnu korpu
        [HttpGet]
        public async Task<ActionResult<ShoppingCartDto>> GetCart()
        {
            var cart = await _db.Carts.Find(c => c.UserId == User.UserId()).FirstOrDefaultAsync();
            if (cart == null)
            {
                cart = new ShoppingCart { UserId = User.UserId() };
                await _db.Carts.InsertOneAsync(cart);
            }

            return Ok(new ShoppingCartDto
            {
                Items = cart.Items.Select(i => new OrderItemDto
                {
                    TourId = i.TourId,
                    TourName = i.TourName,
                    Price = i.Price
                }).ToList(),
                TotalPrice = cart.TotalPrice
            });
        }

        // POST /cart/add → dodaj turu u korpu
        [HttpPost("add")]
        public async Task<ActionResult<ShoppingCartDto>> AddToCart([FromBody] AddToCartRequest req)
        {
            var cart = await _db.Carts.Find(c => c.UserId == User.UserId()).FirstOrDefaultAsync();
            if (cart == null)
            {
                cart = new ShoppingCart { UserId = User.UserId() };
                await _db.Carts.InsertOneAsync(cart);
            }

            cart.Items.Add(new OrderItem
            {
                TourId = req.TourId,
                TourName = req.TourName,
                Price = req.Price
            });

            var update = Builders<ShoppingCart>.Update.Set(c => c.Items, cart.Items);
            await _db.Carts.UpdateOneAsync(c => c.Id == cart.Id, update);

            return Ok(new ShoppingCartDto
            {
                Items = cart.Items.Select(i => new OrderItemDto
                {
                    TourId = i.TourId,
                    TourName = i.TourName,
                    Price = i.Price
                }).ToList(),
                TotalPrice = cart.TotalPrice
            });
        }

        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout([FromServices] CreatePurchaseOrchestrator orchestrator)
        {
            var cart = await _db.Carts.Find(c => c.UserId == User.UserId()).FirstOrDefaultAsync();
            if (cart == null || cart.Items.Count == 0) return BadRequest("Cart is empty.");

            var details = new CPSaga.PurchaseOrderDetails(
                Id: Guid.NewGuid().ToString(),
                UserId: cart.UserId,
                Items: cart.Items
                    .Select(i => new CPSaga.OrderItemDto(i.TourId, i.TourName, i.Price))
                    .ToList()
            );
            await orchestrator.BeginAsync(details);
            // Odmah vraćamo 202 (accepted), a status možeš pratiti u saga logu
            return Accepted(new { saga = details.Id });
        }



    }
}
