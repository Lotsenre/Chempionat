using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;
using VendingMachineAPI.Models.Entities;
using VendingMachineAPI.Utilities;

namespace VendingMachineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? search = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u =>
                u.FullName.Contains(search) ||
                u.Email.Contains(search) ||
                u.Role.Contains(search));
        }

        var total = await query.CountAsync();

        var users = await query
            .OrderBy(u => u.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role,
                IsActive = u.IsActive,
                IsManager = u.IsManager,
                IsEngineer = u.IsEngineer,
                IsOperator = u.IsOperator,
                CompanyId = u.CompanyId,
                FranchiseeCode = u.FranchiseeCode,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            })
            .ToListAsync();

        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return Ok(new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            IsActive = user.IsActive,
            IsManager = user.IsManager,
            IsEngineer = user.IsEngineer,
            IsOperator = user.IsOperator,
            CompanyId = user.CompanyId,
            FranchiseeCode = user.FranchiseeCode,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new { message = "User with this email already exists" });

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true,
            IsManager = request.Role == "Admin",
            IsEngineer = request.Role == "Engineer",
            IsOperator = request.Role == "Franchisee",
            CompanyId = request.CompanyId,
            FranchiseeCode = request.FranchiseeCode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User created: {Email}, Role: {Role}", user.Email, user.Role);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (!string.IsNullOrEmpty(request.FullName))
            user.FullName = request.FullName;
        if (!string.IsNullOrEmpty(request.Email))
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != id))
                return BadRequest(new { message = "Email already taken" });
            user.Email = request.Email;
        }
        if (request.Phone != null)
            user.Phone = request.Phone;
        if (!string.IsNullOrEmpty(request.Role))
        {
            user.Role = request.Role;
            user.IsManager = request.Role == "Admin";
            user.IsEngineer = request.Role == "Engineer";
            user.IsOperator = request.Role == "Franchisee";
        }
        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;
        if (!string.IsNullOrEmpty(request.Password))
            user.PasswordHash = PasswordHasher.HashPassword(request.Password);
        if (request.CompanyId.HasValue)
            user.CompanyId = request.CompanyId.Value == Guid.Empty ? null : request.CompanyId;
        if (request.FranchiseeCode != null)
            user.FranchiseeCode = string.IsNullOrEmpty(request.FranchiseeCode) ? null : request.FranchiseeCode;

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User updated: {Id}", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User deleted: {Id}", id);
        return NoContent();
    }

    [HttpPost("{id}/toggle-active")]
    public async Task<ActionResult> ToggleActive(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { isActive = user.IsActive });
    }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsManager { get; set; }
    public bool IsEngineer { get; set; }
    public bool IsOperator { get; set; }
    public Guid? CompanyId { get; set; }
    public string? FranchiseeCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class CreateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = "Engineer";
    public Guid? CompanyId { get; set; }
    public string? FranchiseeCode { get; set; }
}

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
    public Guid? CompanyId { get; set; }
    public string? FranchiseeCode { get; set; }
}
