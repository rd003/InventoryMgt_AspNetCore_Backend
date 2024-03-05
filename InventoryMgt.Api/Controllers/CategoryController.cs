using InventoryMgt.Data.Models;
using InventoryMgt.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMgt.Api.Controllers;

[ApiController]
[Route("/api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    public CategoryController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    [HttpPost]
    public async Task<IActionResult> CreateCategory(Category category)
    {
        var createdCategory = await _categoryRepository.AddCategory(category);
        return CreatedAtAction(nameof(CreateCategory), createdCategory);
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateCategory(int Id, [FromBody] Category category)
    {
        await _categoryRepository.UpdateCategory(category);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var contegories = await _categoryRepository.GetCategories();
        return Ok(contegories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var contegory = await _categoryRepository.GetCategory(id);
        return Ok(contegory);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _categoryRepository.DeleteCategory(id);
        return NoContent();
    }
}