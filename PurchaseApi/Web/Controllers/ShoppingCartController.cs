using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PurchaseApi.Store;
using PurchaseApi.Store.Model;
using PurchaseApi.Web.Dto;
using PurchaseApi.Web.Auth;

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
        public async Task<ActionResult<CheckoutResponse>> Checkout([FromServices] IHttpClientFactory httpFactory)
        {
            var cart = await _db.Carts.Find(c => c.UserId == User.UserId()).FirstOrDefaultAsync();
            if (cart == null || cart.Items.Count == 0)
                return BadRequest("Cart is empty.");

            var client = httpFactory.CreateClient("ToursApi");

            // 👇 izvuci token iz trenutnog requesta i dodaj ga dalje
            var accessToken = Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.Replace("Bearer ", ""));
            }

            var reserved = new List<string>();

            try
            {
                // 1. Rezerviši sve ture
                foreach (var item in cart.Items)
                {
                    var resp = await client.PostAsync($"/tours/{item.TourId}/reserve", null);
                    if (!resp.IsSuccessStatusCode)
                        throw new Exception($"Reservation failed for tour {item.TourId}");

                    reserved.Add(item.TourId);
                }

                // 2. Ako sve prođe, upiši kupovine
                var tokens = cart.Items.Select(item => new TourPurchaseToken
                {
                    UserId = cart.UserId,
                    TourId = item.TourId
                }).ToList();

                await _db.Purchases.InsertManyAsync(tokens);

                // 3. Potvrdi ture
                foreach (var id in reserved)
                {
                    var resp = await client.PostAsync($"/tours/{id}/confirm", null);
                    if (!resp.IsSuccessStatusCode)
                        throw new Exception($"Confirmation failed for tour {id}");
                }

                // 4. Isprazni korpu
                var update = Builders<ShoppingCart>.Update.Set(c => c.Items, new List<OrderItem>());
                await _db.Carts.UpdateOneAsync(c => c.Id == cart.Id, update);

                return Ok(new CheckoutResponse
                {
                    PurchasedTourIds = tokens.Select(t => t.TourId).ToList(),
                    PurchasedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                // 5. Rollback rezervacija
                foreach (var id in reserved)
                {
                    await client.PostAsync($"/tours/{id}/cancel", null);
                }
                return BadRequest(new { error = ex.Message });
            }
        }


    }
}
