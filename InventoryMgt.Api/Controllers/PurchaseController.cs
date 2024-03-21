
using System.Text.Json;
using InventoryMgt.Api.CustomExceptions;
using InventoryMgt.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMgt.Api.Controllers;

[Route("/api/purchases")]
[ApiController]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseRepository _purchaseRepository;
    public PurchaseController(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePurchase(Purchase purchase)
    {
        var createdPurchaseRecord = await this._purchaseRepository.AddPurchase(purchase);
        return CreatedAtAction(nameof(CreatePurchase), createdPurchaseRecord);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePurchase(int id, [FromBody] Purchase purchase)
    {
        if (id != purchase.Id)
            throw new BadRequestException("id in uri and body does not match");
        var existingPurchase = await _purchaseRepository.GetPurchase(id);
        if (existingPurchase == null)
            throw new NotFoundException($"product with id: {id} does not found");
        var updatedPurchase = await _purchaseRepository.UpdatePurchase(purchase);
        return Ok(updatedPurchase);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPurchase(int id)
    {
        var purchase = await _purchaseRepository.GetPurchase(id);
        if (purchase == null)
            throw new NotFoundException($"product with id: {id} does not found");
        return Ok(purchase);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePurchase(int id)
    {
        var purchase = await _purchaseRepository.GetPurchase(id);
        if (purchase == null)
            throw new NotFoundException($"Record with id: {id} does not found");
        await _purchaseRepository.RemovePurchase(id);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetPurchases(int page = 1, int limit = 4, string? productName = null, DateTime? dateFrom = null, DateTime? dateTo = null, string? sortColumn = null, string? sortDirection = null)
    {
        if (sortDirection != null && !new[] { "asc", "desc" }.Contains(sortDirection))
        {
            throw new BadRequestException("'sortDirection' accepts values 'asc' and 'desc' only");
        }

        var allowedSortColumns = new[] { "Id", "ProductName", "Price", "CreateDate", "UpdateDate", "PurchaseDate" };
        if (sortColumn != null && !allowedSortColumns.Contains(sortColumn))
        {
            throw new BadRequestException($"only {string.Join(',', allowedSortColumns)} columns allowed");
        }
        var paginatedPurchase = await _purchaseRepository.GetPurchases(page, limit, productName, dateFrom, dateTo, sortColumn, sortDirection);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginatedPurchase.Pagination));
        return Ok(paginatedPurchase.Purchases);
    }
}