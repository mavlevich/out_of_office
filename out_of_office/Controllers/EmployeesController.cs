using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using out_of_office.Data;
using out_of_office.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace out_of_office.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly OutOfOfficeContext _context;

    public EmployeesController(OutOfOfficeContext context)
    {
        _context = context;
    }

    // GET: api/Employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(
        [FromQuery] string sortBy = "FullName",
        [FromQuery] bool sortDescending = false,
        [FromQuery] string status = null,
        [FromQuery] string searchName = null)
    {
        IQueryable<Employee> query = _context.Employees;

        // Фильтрация по статусу
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(e => e.Status == status);
        }

        // Поиск по имени
        if (!string.IsNullOrEmpty(searchName))
        {
            query = query.Where(e => e.FullName.Contains(searchName));
        }

        // Применение сортировки в зависимости от параметра sortBy
        query = sortBy.ToLower() switch
        {
            "fullname" => sortDescending ? query.OrderByDescending(e => e.FullName) : query.OrderBy(e => e.FullName),
            "subdivision" => sortDescending ? query.OrderByDescending(e => e.Subdivision) : query.OrderBy(e => e.Subdivision),
            "position" => sortDescending ? query.OrderByDescending(e => e.Position) : query.OrderBy(e => e.Position),
            "status" => sortDescending ? query.OrderByDescending(e => e.Status) : query.OrderBy(e => e.Status),
            _ => sortDescending ? query.OrderByDescending(e => e.FullName) : query.OrderBy(e => e.FullName),
        };

        return await query.ToListAsync();
    }

    // GET: api/Employees/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        return employee;
    }

    // POST: api/Employees
    [HttpPost]
    public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.ID }, employee);
    }

    // PUT: api/Employees/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmployee(int id, Employee employee)
    {
        if (id != employee.ID)
        {
            return BadRequest();
        }

        _context.Entry(employee).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmployeeExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Employees/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EmployeeExists(int id)
    {
        return _context.Employees.Any(e => e.ID == id);
    }
}
