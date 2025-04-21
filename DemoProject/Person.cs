using PDFTemplateLibrary.Attributes;

namespace DemoProject;

public class Person {
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public int Age { get; set; }
    public string IDNumber { get; set; }
    public List<Job> Jobs { get; set; }
    
    [PDFCall] public decimal GetTotalSalary() {
        decimal total = 0;
        Jobs.ForEach(job => total += job.Salary);
        return total;
    }
}