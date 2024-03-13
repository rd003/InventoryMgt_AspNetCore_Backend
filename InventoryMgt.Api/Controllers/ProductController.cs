using System.Text.Json;
using InventoryMgt.Api.CustomExceptions;
using InventoryMgt.Data.Models;
using InventoryMgt.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;

namespace InventoryMgt.Api.Controllers;
[ApiController]
[Route("/api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepo;
    public ProductController(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(Product product)
    {
        var createdProduct = await _productRepo.AddProduct(product);
        return CreatedAtAction(nameof(AddProduct), createdProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
    {
        if (id != product.Id)
            throw new BadRequestException("id in uri and body does not match");
        var existingProduct = await _productRepo.GetProduct(id);
        if (existingProduct == null)
            throw new NotFoundException($"Product with id : {id} does not found");
        var updatedProduct = await _productRepo.UpdatProduct(product);
        return Ok(updatedProduct);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var existingProduct = await _productRepo.GetProduct(id);
        if (existingProduct == null)
            throw new NotFoundException($"Product with id : {id} does not found");
        await _productRepo.DeleteProduct(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productRepo.GetProduct(id);
        if (product == null)
            throw new NotFoundException($"Product with id : {id} does not found");
        return Ok(product);
    }

    [HttpGet()]
    public async Task<IActionResult> GetProducts(int page = 1, int limit = 4, string? searchTerm = null, string? sortColumn = null, string? @sortDirection = null)
    {
        if (sortDirection != null && !new[] { "asc", "desc" }.Contains(sortDirection))
        {
            throw new BadRequestException("'sortDirection' accepts values 'asc' and 'desc' only");
        }

        if (sortColumn != null && !new[] { "Id", "ProductName", "Price", "CreateDate", "UpdateDate", "CategoryName" }.Contains(sortColumn))
        {
            throw new BadRequestException("'sortColumn' accepts values ('Id','ProductName','CreateDate','UpdateDate','Price','CategoryName') only");
        }
        var productResult = await _productRepo.GetProducts(page, limit, searchTerm, sortColumn, sortDirection);
        var products = productResult.Products;
        var paginationHeader = new
        {
            productResult.TotalRecords,
            productResult.TotalPages,
            productResult.Page,
            productResult.Limit,
        };
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));
        return Ok(products);
    }
}