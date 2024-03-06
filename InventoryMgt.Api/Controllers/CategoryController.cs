using InventoryMgt.Api.CustomExceptions;
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
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category categoryToUpdate)
    {
        if (id != categoryToUpdate.Id)
            throw new BadRequestException("You are passing an invalid id");
        var category = await _categoryRepository.GetCategory(id);
        if (category == null)
            throw new NotFoundException($"category with id : {id} does not exists");
        await _categoryRepository.UpdateCategory(categoryToUpdate);
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
        var category = await _categoryRepository.GetCategory(id);
        if (category == null)
            throw new NotFoundException($"category with id : {id} does not exists");
        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _categoryRepository.GetCategory(id);
        if (category == null)
            throw new NotFoundException($"category with id : {id} does not exists");
        await _categoryRepository.DeleteCategory(id);
        return NoContent();
    }
}