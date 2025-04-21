using PDFTemplateLibrary.Attributes;

namespace Application;

public class Person {
    public string Name { get; set; }
    public string Surname { get; set; }
    public int Age { get; set; }
    public string ID_Number { get; set; }
    public List<Job> Jobs { get; set; }
    
    [PDFCall] public decimal GetTotalSalary() {
        decimal total = 0;
        Jobs.ForEach(job => total += job.Salary);
        return total;
    }
}