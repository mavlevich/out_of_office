namespace out_of_office.Models;

public class LeaveRequest
{
    public int ID { get; set; }
    public int EmployeeID { get; set; }
    public string AbsenceReason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }
}