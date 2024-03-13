using InventoryMgt.Api.CustomExceptions;
using InventoryMgt.Data.Models;
using InventoryMgt.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productRepo.GetProducts();
        return Ok(products);
    }
}