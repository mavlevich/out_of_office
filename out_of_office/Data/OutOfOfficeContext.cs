using Microsoft.EntityFrameworkCore;
using out_of_office.Models;

namespace out_of_office.Data;

public class OutOfOfficeContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
    public DbSet<Project> Projects { get; set; }

    public OutOfOfficeContext(DbContextOptions<OutOfOfficeContext> options)
        : base(options)
    {
    }
}