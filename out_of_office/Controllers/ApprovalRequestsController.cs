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
    // [Authorize(Roles = RoleModel.HRManager + "," + RoleModel.ProjectManager)]
    public class ApprovalRequestsController : ControllerBase
    {
        private readonly OutOfOfficeContext _context;

        public ApprovalRequestsController(OutOfOfficeContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApprovalRequest>>> GetApprovalRequests(
            string sortBy = "Status",
            string sortOrder = "asc")
        {
            var approvalRequests = _context.ApprovalRequests.AsQueryable();

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortOrder.ToLower() == "desc")
                {
                    approvalRequests = approvalRequests.OrderByDescending(ar => EF.Property<object>(ar, sortBy));
                }
                else
                {
                    approvalRequests = approvalRequests.OrderBy(ar => EF.Property<object>(ar, sortBy));
                }
            }

            return await approvalRequests.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApprovalRequest>> GetApprovalRequest(int id)
        {
            var approvalRequest = await _context.ApprovalRequests.FindAsync(id);

            if (approvalRequest == null)
            {
                return NotFound();
            }

            return approvalRequest;
        }

        [HttpPost]
        public async Task<ActionResult<ApprovalRequest>> PostApprovalRequest(ApprovalRequest approvalRequest)
        {
            _context.ApprovalRequests.Add(approvalRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApprovalRequest), new { id = approvalRequest.ID }, approvalRequest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutApprovalRequest(int id, ApprovalRequest approvalRequest)
        {
            if (id != approvalRequest.ID)
            {
                return BadRequest();
            }

            _context.Entry(approvalRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApprovalRequestExists(id))
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
        public async Task<IActionResult> DeleteApprovalRequest(int id)
        {
            var approvalRequest = await _context.ApprovalRequests.FindAsync(id);
            if (approvalRequest == null)
            {
                return NotFound();
            }

            _context.ApprovalRequests.Remove(approvalRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApprovalRequestExists(int id)
        {
            return _context.ApprovalRequests.Any(e => e.ID == id);
        }
    }
}
