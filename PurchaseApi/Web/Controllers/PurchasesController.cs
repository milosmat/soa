using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PurchaseApi.Store;
using PurchaseApi.Store.Model;
using PurchaseApi.Web.Dto;
using PurchaseApi.Web.Auth; // isti UserId helper kao u drugim servisima

namespace PurchaseApi.Web.Controllers
{
    [ApiController]
    [Authorize]
    public class PurchasesController : ControllerBase
    {
        private readonly AppDb _db;
        public PurchasesController(AppDb db) => _db = db;

        [HttpGet("purchases/mine")]
        public async Task<ActionResult<IEnumerable<TourPurchaseToken>>> MyPurchases()
        {
            var tokens = await _db.Purchases
                .Find(p => p.UserId == User.UserId())
                .ToListAsync();

            return Ok(tokens);
        }

    }
}
