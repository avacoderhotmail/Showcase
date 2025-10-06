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
    private readonly IBlobService _blobService;

    public ProductsController(IProductService service, IBlobService blobService)
    {
        _service = service;
        _blobService = blobService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAll()
    { 
        var products = await _service.GetAllAsync();
        return Ok(products);
    }

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

    //[HttpPost("upload")]
    //[Authorize(Roles = "Admin,ProductManager")]
    //public async Task<IActionResult> Upload(IFormFile file)
    //{
    //    if (file == null || file.Length == 0)
    //        return BadRequest("No file uploaded.");

    //    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
    //    var uri = await _blobService.UploadAsync(file, fileName);

    //    return Ok(new { fileName, uri });
    //}

    [HttpGet("image/{fileName}")]
    [Authorize(Roles = "Admin,User")]
    public IActionResult GetImage(string fileName)
    {
        var sasUri = _blobService.GetSasUri(fileName, 60); // 1 hour
        return Ok(new { uri = sasUri });
    }

}
