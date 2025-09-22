using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Showcase.Contracts.Contracts.Product;
using Showcase.Application.Interfaces;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,ProductManager")]
    public async Task<IActionResult> Create([FromForm] ProductCreateDto dto, IFormFile? imageFile)
    {
        var product = await _service.CreateAsync(dto, imageFile);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, ProductManager")]
    public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateDto dto, IFormFile? imageFile)
    {
        var product = await _service.UpdateAsync(id, dto, imageFile);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin, ProductManager")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
