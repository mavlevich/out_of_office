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
    // [Authorize(Roles = RoleModel.Employee + "," + RoleModel.HRManager + "," + RoleModel.ProjectManager)]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly OutOfOfficeContext _context;

        public LeaveRequestsController(OutOfOfficeContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = RoleModel.HRManager + "," + RoleModel.ProjectManager)]
        public async Task<ActionResult<IEnumerable<LeaveRequest>>> GetLeaveRequests()
        {
            return await _context.LeaveRequests.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleModel.HRManager + "," + RoleModel.ProjectManager)]
        public async Task<ActionResult<LeaveRequest>> GetLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            return leaveRequest;
        }

        [HttpPost]
        [Authorize(Roles = RoleModel.Employee)]
        public async Task<ActionResult<LeaveRequest>> PostLeaveRequest(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.ID }, leaveRequest);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleModel.HRManager + "," + RoleModel.ProjectManager)]
        public async Task<IActionResult> PutLeaveRequest(int id, LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.ID)
            {
                return BadRequest();
            }

            _context.Entry(leaveRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveRequestExists(id))
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
        [Authorize(Roles = RoleModel.HRManager + "," + RoleModel.ProjectManager)]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaveRequestExists(int id)
        {
            return _context.LeaveRequests.Any(e => e.ID == id);
        }
    }
}
