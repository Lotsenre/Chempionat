using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;

namespace VendingMachineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("sales")]
    public async Task<ActionResult> GetSalesReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] Guid? vendingMachineId = null)
    {
        var fromDate = DateTime.SpecifyKind(from ?? DateTime.UtcNow.AddDays(-30), DateTimeKind.Utc);
        var toDate = DateTime.SpecifyKind(to ?? DateTime.UtcNow, DateTimeKind.Utc);

        var query = _context.Sales
            .Include(s => s.Product)
            .Include(s => s.VendingMachine)
            .Where(s => s.Timestamp >= fromDate && s.Timestamp <= toDate);

        if (vendingMachineId.HasValue)
            query = query.Where(s => s.VendingMachineId == vendingMachineId);

        var sales = await query
            .OrderByDescending(s => s.Timestamp)
            .Select(s => new
            {
                s.Id,
                s.Timestamp,
                ProductName = s.Product != null ? s.Product.Name : "N/A",
                MachineName = s.VendingMachine != null ? s.VendingMachine.Name : "N/A",
                MachineSerial = s.VendingMachine != null ? s.VendingMachine.SerialNumber : "N/A",
                s.Quantity,
                s.TotalPrice,
                s.PaymentMethod
            })
            .ToListAsync();

        var totalRevenue = sales.Sum(s => s.TotalPrice);
        var totalQuantity = sales.Sum(s => s.Quantity);
        var avgCheck = sales.Count > 0 ? totalRevenue / sales.Count : 0;

        return Ok(new
        {
            Summary = new
            {
                TotalSales = sales.Count,
                TotalRevenue = totalRevenue,
                TotalQuantity = totalQuantity,
                AverageCheck = Math.Round(avgCheck, 2),
                Period = new { From = fromDate, To = toDate }
            },
            Data = sales
        });
    }

    [HttpGet("maintenance")]
    public async Task<ActionResult> GetMaintenanceReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] Guid? vendingMachineId = null)
    {
        var fromDate = DateTime.SpecifyKind(from ?? DateTime.UtcNow.AddDays(-30), DateTimeKind.Utc);
        var toDate = DateTime.SpecifyKind(to ?? DateTime.UtcNow, DateTimeKind.Utc);

        var query = _context.MaintenanceRecords
            .Include(m => m.VendingMachine)
            .Where(m => m.Date >= fromDate && m.Date <= toDate);

        if (vendingMachineId.HasValue)
            query = query.Where(m => m.VendingMachineId == vendingMachineId);

        var records = await query
            .OrderByDescending(m => m.Date)
            .Select(m => new
            {
                m.Id,
                m.Date,
                MachineName = m.VendingMachine.Name,
                MachineSerial = m.VendingMachine.SerialNumber,
                m.WorkDescription,
                m.IssuesFound,
                TechnicianName = m.FullName,
                m.Status
            })
            .ToListAsync();

        var completed = records.Count(r => r.Status == "Completed");
        var pending = records.Count(r => r.Status == "Pending");

        return Ok(new
        {
            Summary = new
            {
                Total = records.Count,
                Completed = completed,
                Pending = pending,
                Period = new { From = fromDate, To = toDate }
            },
            Data = records
        });
    }

    [HttpGet("revenue")]
    public async Task<ActionResult> GetRevenueReport(
        [FromQuery] int days = 30,
        [FromQuery] Guid? vendingMachineId = null)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);

        var query = _context.Sales.Where(s => s.Timestamp >= startDate);
        if (vendingMachineId.HasValue)
            query = query.Where(s => s.VendingMachineId == vendingMachineId);

        var dailyRevenue = await query
            .GroupBy(s => s.Timestamp.Date)
            .Select(g => new
            {
                Date = g.Key,
                Revenue = g.Sum(s => s.TotalPrice),
                SalesCount = g.Count(),
                Quantity = g.Sum(s => s.Quantity)
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var byMachine = await _context.Sales
            .Where(s => s.Timestamp >= startDate)
            .GroupBy(s => new { s.VendingMachineId, s.VendingMachine!.Name })
            .Select(g => new
            {
                MachineId = g.Key.VendingMachineId,
                MachineName = g.Key.Name,
                Revenue = g.Sum(s => s.TotalPrice),
                SalesCount = g.Count()
            })
            .OrderByDescending(x => x.Revenue)
            .ToListAsync();

        var byPayment = await query
            .GroupBy(s => s.PaymentMethod)
            .Select(g => new
            {
                Method = g.Key,
                Revenue = g.Sum(s => s.TotalPrice),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Revenue)
            .ToListAsync();

        return Ok(new
        {
            DailyRevenue = dailyRevenue,
            ByMachine = byMachine,
            ByPaymentMethod = byPayment
        });
    }
}
