namespace VendingMachineDesktop.Models.DTOs;

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
