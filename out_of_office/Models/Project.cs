namespace out_of_office.Models;

public class Project
{
    public int ID { get; set; }
    public string ProjectType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int ProjectManagerID { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }
}