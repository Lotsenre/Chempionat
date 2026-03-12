using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;
using VendingMachineAPI.Models.Entities;

namespace VendingMachineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContractsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ContractsController> _logger;

    public ContractsController(ApplicationDbContext context, ILogger<ContractsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContractDto>>> GetAll([FromQuery] string? status = null)
    {
        var query = _context.Contracts
            .Include(c => c.Company)
            .Include(c => c.VendingMachine)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(c => c.Status == status);

        var contracts = await query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new ContractDto
            {
                Id = c.Id,
                ContractNumber = c.ContractNumber,
                CompanyId = c.CompanyId,
                CompanyName = c.Company != null ? c.Company.Name : null,
                VendingMachineId = c.VendingMachineId,
                VendingMachineName = c.VendingMachine.Name,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                MonthlyRent = c.MonthlyRent,
                YearlyRent = c.YearlyRent,
                Status = c.Status,
                ManagementType = c.ManagementType,
                InsuranceRequired = c.InsuranceRequired,
                SignedAt = c.SignedAt,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(contracts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContractDto>> GetById(Guid id)
    {
        var c = await _context.Contracts
            .Include(c => c.Company)
            .Include(c => c.VendingMachine)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (c == null) return NotFound();

        return Ok(new ContractDto
        {
            Id = c.Id,
            ContractNumber = c.ContractNumber,
            CompanyId = c.CompanyId,
            CompanyName = c.Company?.Name,
            VendingMachineId = c.VendingMachineId,
            VendingMachineName = c.VendingMachine.Name,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            MonthlyRent = c.MonthlyRent,
            YearlyRent = c.YearlyRent,
            Status = c.Status,
            ManagementType = c.ManagementType,
            InsuranceRequired = c.InsuranceRequired,
            SignedAt = c.SignedAt,
            CreatedAt = c.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<ContractDto>> Create([FromBody] CreateContractRequest request)
    {
        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            ContractNumber = request.ContractNumber ?? $"C-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            CompanyId = request.CompanyId,
            VendingMachineId = request.VendingMachineId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MonthlyRent = request.MonthlyRent,
            YearlyRent = request.YearlyRent,
            PaybackPeriodMonths = request.PaybackPeriodMonths,
            InsuranceRequired = request.InsuranceRequired,
            ManagementType = request.ManagementType,
            Status = "Draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Contract created: {Number}", contract.ContractNumber);
        return CreatedAtAction(nameof(GetById), new { id = contract.Id }, new ContractDto
        {
            Id = contract.Id,
            ContractNumber = contract.ContractNumber,
            Status = contract.Status,
            CreatedAt = contract.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateContractRequest request)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null) return NotFound();

        if (request.StartDate.HasValue)
            contract.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue)
            contract.EndDate = request.EndDate.Value;
        if (request.MonthlyRent.HasValue)
            contract.MonthlyRent = request.MonthlyRent.Value;
        if (request.YearlyRent.HasValue)
            contract.YearlyRent = request.YearlyRent.Value;
        if (!string.IsNullOrEmpty(request.Status))
            contract.Status = request.Status;
        if (request.ManagementType != null)
            contract.ManagementType = request.ManagementType;
        if (request.InsuranceRequired.HasValue)
            contract.InsuranceRequired = request.InsuranceRequired.Value;

        contract.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Contract updated: {Id}", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null) return NotFound();

        _context.Contracts.Remove(contract);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Contract deleted: {Id}", id);
        return NoContent();
    }

    [HttpPost("{id}/sign")]
    public async Task<ActionResult> Sign(Guid id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null) return NotFound();

        contract.Status = "Signed";
        contract.SignedAt = DateTime.UtcNow;
        contract.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Contract signed: {Id}", id);
        return Ok(new { message = "Contract signed", signedAt = contract.SignedAt });
    }
}

public class ContractDto
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public Guid VendingMachineId { get; set; }
    public string VendingMachineName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal YearlyRent { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ManagementType { get; set; }
    public bool InsuranceRequired { get; set; }
    public DateTime? SignedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateContractRequest
{
    public string? ContractNumber { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid VendingMachineId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal YearlyRent { get; set; }
    public int? PaybackPeriodMonths { get; set; }
    public bool InsuranceRequired { get; set; }
    public string? ManagementType { get; set; }
}

public class UpdateContractRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MonthlyRent { get; set; }
    public decimal? YearlyRent { get; set; }
    public string? Status { get; set; }
    public string? ManagementType { get; set; }
    public bool? InsuranceRequired { get; set; }
}
