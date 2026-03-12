using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;
using VendingMachineAPI.Models.Entities;

namespace VendingMachineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ModemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ModemsController> _logger;

    public ModemsController(ApplicationDbContext context, ILogger<ModemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModemDto>>> GetAll([FromQuery] string? search = null)
    {
        var query = _context.Modems.Include(m => m.VendingMachine).AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(m =>
                m.ModemNumber.Contains(search) ||
                (m.Model != null && m.Model.Contains(search)));
        }

        var modems = await query
            .OrderBy(m => m.ModemNumber)
            .Select(m => new ModemDto
            {
                Id = m.Id,
                ModemNumber = m.ModemNumber,
                Model = m.Model,
                Status = m.Status,
                VendingMachineId = m.VendingMachineId,
                VendingMachineName = m.VendingMachine != null ? m.VendingMachine.Name : null,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();

        return Ok(modems);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ModemDto>> GetById(Guid id)
    {
        var modem = await _context.Modems.Include(m => m.VendingMachine).FirstOrDefaultAsync(m => m.Id == id);
        if (modem == null) return NotFound();

        return Ok(new ModemDto
        {
            Id = modem.Id,
            ModemNumber = modem.ModemNumber,
            Model = modem.Model,
            Status = modem.Status,
            VendingMachineId = modem.VendingMachineId,
            VendingMachineName = modem.VendingMachine?.Name,
            CreatedAt = modem.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<ModemDto>> Create([FromBody] CreateModemRequest request)
    {
        if (await _context.Modems.AnyAsync(m => m.ModemNumber == request.ModemNumber))
            return BadRequest(new { message = "Modem with this number already exists" });

        var modem = new Modem
        {
            Id = Guid.NewGuid(),
            ModemNumber = request.ModemNumber,
            Model = request.Model,
            Status = request.Status ?? "Active",
            VendingMachineId = request.VendingMachineId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Modems.Add(modem);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Modem created: {ModemNumber}", modem.ModemNumber);
        return CreatedAtAction(nameof(GetById), new { id = modem.Id }, new ModemDto
        {
            Id = modem.Id,
            ModemNumber = modem.ModemNumber,
            Model = modem.Model,
            Status = modem.Status,
            VendingMachineId = modem.VendingMachineId,
            CreatedAt = modem.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateModemRequest request)
    {
        var modem = await _context.Modems.FindAsync(id);
        if (modem == null) return NotFound();

        if (!string.IsNullOrEmpty(request.ModemNumber))
            modem.ModemNumber = request.ModemNumber;
        if (request.Model != null)
            modem.Model = request.Model;
        if (!string.IsNullOrEmpty(request.Status))
            modem.Status = request.Status;
        if (request.VendingMachineId.HasValue)
            modem.VendingMachineId = request.VendingMachineId.Value == Guid.Empty ? null : request.VendingMachineId;

        modem.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Modem updated: {Id}", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var modem = await _context.Modems.FindAsync(id);
        if (modem == null) return NotFound();

        _context.Modems.Remove(modem);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Modem deleted: {Id}", id);
        return NoContent();
    }

    [HttpPost("{id}/attach/{machineId}")]
    public async Task<ActionResult> AttachToMachine(Guid id, Guid machineId)
    {
        var modem = await _context.Modems.FindAsync(id);
        if (modem == null) return NotFound(new { message = "Modem not found" });

        var machine = await _context.VendingMachines.FindAsync(machineId);
        if (machine == null) return NotFound(new { message = "Vending machine not found" });

        modem.VendingMachineId = machineId;
        modem.UpdatedAt = DateTime.UtcNow;
        machine.KitOnlineId = modem.ModemNumber;
        machine.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Modem {ModemId} attached to machine {MachineId}", id, machineId);
        return Ok(new { message = "Modem attached" });
    }

    [HttpPost("{id}/detach")]
    public async Task<ActionResult> Detach(Guid id)
    {
        var modem = await _context.Modems.FindAsync(id);
        if (modem == null) return NotFound();

        if (modem.VendingMachineId.HasValue)
        {
            var machine = await _context.VendingMachines.FindAsync(modem.VendingMachineId);
            if (machine != null)
            {
                machine.KitOnlineId = null;
                machine.UpdatedAt = DateTime.UtcNow;
            }
        }

        modem.VendingMachineId = null;
        modem.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Modem detached: {Id}", id);
        return NoContent();
    }
}

public class ModemDto
{
    public Guid Id { get; set; }
    public string ModemNumber { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string Status { get; set; } = "Active";
    public Guid? VendingMachineId { get; set; }
    public string? VendingMachineName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateModemRequest
{
    public string ModemNumber { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Status { get; set; }
    public Guid? VendingMachineId { get; set; }
}

public class UpdateModemRequest
{
    public string? ModemNumber { get; set; }
    public string? Model { get; set; }
    public string? Status { get; set; }
    public Guid? VendingMachineId { get; set; }
}
