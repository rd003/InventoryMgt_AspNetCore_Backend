
using Microsoft.AspNetCore.Mvc;

namespace InventoryMgt.Api.Controllers;

[Route("/api/purchases")]
[ApiController]
public class PurchaseController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPurchases()
    {
        return Ok("maja aa gaya");
    }
}