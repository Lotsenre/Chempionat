using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;
using VendingMachineAPI.Models.Entities;

namespace VendingMachineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(ApplicationDbContext context, ILogger<CompaniesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/companies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies()
    {
        try
        {
            var companies = await _context.Companies
                .Include(c => c.ParentCompany)
                .OrderBy(c => c.Name)
                .Select(c => new CompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Address = c.Address,
                    ContactInfo = c.ContactInfo ?? string.Empty,
                    Notes = c.Notes ?? string.Empty,
                    ParentCompanyId = c.ParentCompanyId,
                    ParentCompanyName = c.ParentCompany != null ? c.ParentCompany.Name : string.Empty,
                    WorkingSince = c.WorkingSince,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(companies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting companies");
            return StatusCode(500, "Internal server error");
        }
    }

    // GET: api/companies/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetCompany(Guid id)
    {
        var company = await _context.Companies
            .Include(c => c.ParentCompany)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
        {
            return NotFound();
        }

        return Ok(new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Address = company.Address,
            ContactInfo = company.ContactInfo ?? string.Empty,
            Notes = company.Notes ?? string.Empty,
            ParentCompanyId = company.ParentCompanyId,
            ParentCompanyName = company.ParentCompany?.Name ?? string.Empty,
            WorkingSince = company.WorkingSince,
            CreatedAt = company.CreatedAt
        });
    }

    // POST: api/companies
    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany(CompanyDto dto)
    {
        try
        {
            var company = new Company
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Name = dto.Name,
                Address = dto.Address,
                ContactInfo = dto.ContactInfo,
                Notes = dto.Notes,
                ParentCompanyId = dto.ParentCompanyId,
                WorkingSince = dto.WorkingSince == default ? DateTime.UtcNow : dto.WorkingSince,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Reload with parent
            await _context.Entry(company).Reference(c => c.ParentCompany).LoadAsync();

            var result = new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Address = company.Address,
                ContactInfo = company.ContactInfo ?? string.Empty,
                Notes = company.Notes ?? string.Empty,
                ParentCompanyId = company.ParentCompanyId,
                ParentCompanyName = company.ParentCompany?.Name ?? string.Empty,
                WorkingSince = company.WorkingSince,
                CreatedAt = company.CreatedAt
            };

            return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company");
            return StatusCode(500, "Internal server error");
        }
    }

    // PUT: api/companies/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(Guid id, CompanyDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("ID mismatch");
        }

        var company = await _context.Companies.FindAsync(id);
        if (company == null)
        {
            return NotFound();
        }

        company.Name = dto.Name;
        company.Address = dto.Address;
        company.ContactInfo = dto.ContactInfo;
        company.Notes = dto.Notes;
        company.ParentCompanyId = dto.ParentCompanyId;
        company.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Companies.AnyAsync(c => c.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/companies/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
        {
            return NotFound();
        }

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/companies/seed
    [HttpPost("seed")]
    public async Task<ActionResult> SeedCompanies()
    {
        try
        {
            // Check if companies already exist
            if (await _context.Companies.AnyAsync())
            {
                return Ok("Companies already seeded");
            }

            // Create parent company
            var parentCompany = new Company
            {
                Id = Guid.NewGuid(),
                Name = "ООО Торговые Автоматы",
                Address = "г. Москва, ул. Центральная, д. 1",
                ContactInfo = "+7 (495) 123-45-67",
                Notes = "Головная компания",
                WorkingSince = new DateTime(2020, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(parentCompany);
            await _context.SaveChangesAsync();

            // Create child companies
            var childCompanies = new List<Company>
            {
                new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "ИП Иванов А.А.",
                    Address = "г. Санкт-Петербург, пр. Невский, д. 100",
                    ContactInfo = "+7 (812) 987-65-43",
                    Notes = "Франчайзи СПб",
                    ParentCompanyId = parentCompany.Id,
                    WorkingSince = new DateTime(2021, 3, 20, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "ООО ВендингПро",
                    Address = "г. Казань, ул. Баумана, д. 50",
                    ContactInfo = "+7 (843) 555-12-34",
                    Notes = "Региональный партнер",
                    ParentCompanyId = parentCompany.Id,
                    WorkingSince = new DateTime(2022, 6, 10, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "АО Автоматика Плюс",
                    Address = "г. Новосибирск, ул. Красный проспект, д. 25",
                    ContactInfo = "+7 (383) 222-33-44",
                    Notes = "Сибирский филиал",
                    ParentCompanyId = parentCompany.Id,
                    WorkingSince = new DateTime(2023, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "ИП Петрова М.С.",
                    Address = "г. Екатеринбург, ул. Ленина, д. 15",
                    ContactInfo = "+7 (343) 111-22-33",
                    Notes = "Уральский регион",
                    ParentCompanyId = parentCompany.Id,
                    WorkingSince = new DateTime(2023, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "ООО Кофемат",
                    Address = "г. Нижний Новгород, ул. Большая Покровская, д. 35",
                    ContactInfo = "+7 (831) 444-55-66",
                    Notes = "Кофейные автоматы",
                    ParentCompanyId = parentCompany.Id,
                    WorkingSince = new DateTime(2021, 5, 12, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "ИП Сидоров В.П.",
                    Address = "г. Самара, ул. Куйбышева, д. 88",
                    ContactInfo = "+7 (846) 333-22-11",
                    Notes = "Поволжский регион",
                    ParentCompanyId = parentCompany.Id,
                    WorkingSince = new DateTime(2022, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "ООО СнекМастер",
                    Address = "г. Ростов-на-Дону, пр. Ворошиловский, д. 12",
                    ContactInfo = "+7 (863) 777-88-99",
                    Notes = "Снековые автоматы ЮФО",
                    ParentCompanyId = parentCompany.Id,
                    WorkingSince = new DateTime(2023, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "АО Вендинг Сервис",
                    Address = "г. Краснодар, ул. Красная, д. 150",
                    ContactInfo = "+7 (861) 200-30-40",
                    Notes = "Техобслуживание автоматов",
                    ParentCompanyId = parentCompany.Id,
                    WorkingSince = new DateTime(2022, 4, 20, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "ИП Козлова Е.И.",
                    Address = "г. Воронеж, ул. Плехановская, д. 45",
                    ContactInfo = "+7 (473) 500-60-70",
                    Notes = "Центральный регион",
                    ParentCompanyId = parentCompany.Id,
                    WorkingSince = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _context.Companies.AddRange(childCompanies);
            await _context.SaveChangesAsync();

            return Ok($"Seeded {childCompanies.Count + 1} companies");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding companies");
            return StatusCode(500, $"Error seeding: {ex.Message}");
        }
    }
}

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public Guid? ParentCompanyId { get; set; }
    public string ParentCompanyName { get; set; } = string.Empty;
    public DateTime WorkingSince { get; set; }
    public DateTime CreatedAt { get; set; }
}
