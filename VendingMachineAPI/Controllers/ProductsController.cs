using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;
using VendingMachineAPI.Models.Entities;

namespace VendingMachineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? search = null,
        [FromQuery] Guid? vendingMachineId = null,
        [FromQuery] bool? lowStock = null)
    {
        var query = _context.Products.Include(p => p.VendingMachine).AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search) || (p.Category != null && p.Category.Contains(search)));

        if (vendingMachineId.HasValue)
            query = query.Where(p => p.VendingMachineId == vendingMachineId);

        if (lowStock == true)
            query = query.Where(p => p.QuantityAvailable <= p.MinStock);

        var total = await query.CountAsync();

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                MinStock = p.MinStock,
                Category = p.Category,
                VendingMachineId = p.VendingMachineId,
                VendingMachineName = p.VendingMachine != null ? p.VendingMachine.Name : null,
                QuantityAvailable = p.QuantityAvailable,
                SalesTrend = p.SalesTrend,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var p = await _context.Products.Include(p => p.VendingMachine).FirstOrDefaultAsync(p => p.Id == id);
        if (p == null) return NotFound();

        return Ok(new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            MinStock = p.MinStock,
            Category = p.Category,
            VendingMachineId = p.VendingMachineId,
            VendingMachineName = p.VendingMachine?.Name,
            QuantityAvailable = p.QuantityAvailable,
            SalesTrend = p.SalesTrend,
            CreatedAt = p.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            MinStock = request.MinStock,
            Category = request.Category,
            VendingMachineId = request.VendingMachineId,
            QuantityAvailable = request.QuantityAvailable,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product created: {Name}", product.Name);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category,
            QuantityAvailable = product.QuantityAvailable,
            CreatedAt = product.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        if (!string.IsNullOrEmpty(request.Name))
            product.Name = request.Name;
        if (request.Description != null)
            product.Description = request.Description;
        if (request.Price.HasValue)
            product.Price = request.Price.Value;
        if (request.MinStock.HasValue)
            product.MinStock = request.MinStock.Value;
        if (request.Category != null)
            product.Category = request.Category;
        if (request.VendingMachineId.HasValue)
            product.VendingMachineId = request.VendingMachineId.Value == Guid.Empty ? null : request.VendingMachineId;
        if (request.QuantityAvailable.HasValue)
            product.QuantityAvailable = request.QuantityAvailable.Value;

        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product updated: {Id}", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product deleted: {Id}", id);
        return NoContent();
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        var categories = await _context.Products
            .Where(p => p.Category != null)
            .Select(p => p.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return Ok(categories);
    }
}

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int MinStock { get; set; }
    public string? Category { get; set; }
    public Guid? VendingMachineId { get; set; }
    public string? VendingMachineName { get; set; }
    public int QuantityAvailable { get; set; }
    public decimal? SalesTrend { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int MinStock { get; set; } = 10;
    public string? Category { get; set; }
    public Guid? VendingMachineId { get; set; }
    public int QuantityAvailable { get; set; }
}

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? MinStock { get; set; }
    public string? Category { get; set; }
    public Guid? VendingMachineId { get; set; }
    public int? QuantityAvailable { get; set; }
}
