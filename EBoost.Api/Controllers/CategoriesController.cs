using EBoost.Application.DTOs.Categories;
using EBoost.Application.Interfaces.Repositories;
using EBoost.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;

namespace EBoost.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _repo;
    private readonly IMapper _mapper;

    public CategoriesController(ICategoryRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    //Add Category
    [Authorize(Policy = "AdminOnly")]
    [HttpPost("AddCategory")]
    public async Task<IActionResult> Create([FromForm]CreateCategoryDto dto)
    {
        if (await _repo.ExistsAsync(dto.Name))
            return BadRequest("Category already exists");

        var category = new Category
        {
            Name = dto.Name.Trim()
        };

        await _repo.AddAsync(category);
        await _repo.SaveChangesAsync();

        return Ok("Category created");
    }

    //GetAll
    [HttpGet("AllCategories")]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _repo.GetAllAsync();

        var result = categories.Select(c =>
            new CategoryDto(c.Id, c.Name, c.IsActive));

        return Ok(result); 
    }

    //GetByID
    [HttpGet("CategoryById")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        
        if(category == null) return NotFound("Category not Found");

        var result = _mapper.Map<CategoryDto>(category);

        return Ok(result);
    }

    //Update
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("UpdateCategory")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCategoryDto dto)
    {
        var category = await _repo.GetByIdAsync(id);

        if (category == null)
            return NotFound("Category not found");

        if (await _repo.ExistsAsync(dto.Name))
            return BadRequest("Category name already exists");

        category.Name = dto.Name.Trim();

        await _repo.SaveChangesAsync();

        return Ok("Category updated");
    }

    //Toggle Active 
    [Authorize(Policy = "AdminOnly")]
    [HttpPatch("Categorystatus")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var category = await _repo.GetByIdAsync(id);

        if (category == null)
            return NotFound("Category not found");

        category.IsActive = !category.IsActive;

        await _repo.SaveChangesAsync();

        return Ok($"Category is now {(category.IsActive ? "Active" : "Inactive")}");
    }
}
