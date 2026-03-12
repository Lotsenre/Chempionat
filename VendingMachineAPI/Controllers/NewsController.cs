using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;
using VendingMachineAPI.Models.Entities;

namespace VendingMachineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NewsController> _logger;

    public NewsController(ApplicationDbContext context, ILogger<NewsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewsDto>>> GetAll([FromQuery] bool? activeOnly = null)
    {
        var query = _context.News.Include(n => n.Author).AsQueryable();

        if (activeOnly == true)
            query = query.Where(n => n.IsActive);

        var news = await query
            .OrderByDescending(n => n.PublishedAt)
            .Select(n => new NewsDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                PublishedAt = n.PublishedAt,
                AuthorId = n.AuthorId,
                AuthorName = n.Author.FullName,
                IsActive = n.IsActive
            })
            .ToListAsync();

        return Ok(news);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NewsDto>> GetById(Guid id)
    {
        var n = await _context.News.Include(n => n.Author).FirstOrDefaultAsync(n => n.Id == id);
        if (n == null) return NotFound();

        return Ok(new NewsDto
        {
            Id = n.Id,
            Title = n.Title,
            Content = n.Content,
            PublishedAt = n.PublishedAt,
            AuthorId = n.AuthorId,
            AuthorName = n.Author.FullName,
            IsActive = n.IsActive
        });
    }

    [HttpPost]
    public async Task<ActionResult<NewsDto>> Create([FromBody] CreateNewsRequest request)
    {
        var news = new News
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            AuthorId = request.AuthorId,
            PublishedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.News.Add(news);
        await _context.SaveChangesAsync();

        _logger.LogInformation("News created: {Title}", news.Title);
        return CreatedAtAction(nameof(GetById), new { id = news.Id }, new NewsDto
        {
            Id = news.Id,
            Title = news.Title,
            Content = news.Content,
            PublishedAt = news.PublishedAt,
            AuthorId = news.AuthorId,
            IsActive = news.IsActive
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateNewsRequest request)
    {
        var news = await _context.News.FindAsync(id);
        if (news == null) return NotFound();

        if (!string.IsNullOrEmpty(request.Title))
            news.Title = request.Title;
        if (request.Content != null)
            news.Content = request.Content;
        if (request.IsActive.HasValue)
            news.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();

        _logger.LogInformation("News updated: {Id}", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var news = await _context.News.FindAsync(id);
        if (news == null) return NotFound();

        _context.News.Remove(news);
        await _context.SaveChangesAsync();

        _logger.LogInformation("News deleted: {Id}", id);
        return NoContent();
    }
}

public class NewsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateNewsRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
}

public class UpdateNewsRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool? IsActive { get; set; }
}
