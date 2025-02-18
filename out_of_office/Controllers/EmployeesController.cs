using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using out_of_office.Data;
using out_of_office.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace out_of_office.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = RoleModel.HRManager + "," + RoleModel.Administrator)]
    public class EmployeesController : ControllerBase
    {
        private readonly OutOfOfficeContext _context;

        public EmployeesController(OutOfOfficeContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(
            string sortBy = "Full Name",
            string sortOrder = "asc")
        {
            var employees = _context.Employees.AsQueryable();

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortOrder.ToLower() == "desc")
                {
                    employees = employees.OrderByDescending(e => EF.Property<object>(e, sortBy));
                }
                else
                {
                    employees = employees.OrderBy(e => EF.Property<object>(e, sortBy));
                }
            }

            return await employees.ToListAsync();
        }

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

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.ID }, employee);
        }

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
}
