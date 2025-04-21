using PDFTemplateLibrary;

namespace Application;

class Program {
    public static void Main() {
        Person person = new() {
            Name = "Cameron",
            Surname = "Stocks",
            Age = 22,
            ID_Number = "1234567890123",
            Jobs = [
                new Job { CompanyName = "Test Company 1", Description = "Test Description 1", TimeAtJob = "2 Years", Salary = 5000, Title = "Worker"},
                new Job { CompanyName = "Test Company 2", Description = "Test Description 2", TimeAtJob = "1 Year", Salary = 5000, Title = "Worker"},
                new Job { CompanyName = "Test Company 3", Description = "Test Description 3", TimeAtJob = "1 Year 5 Months", Salary = 5000, Title = "Worker"},
                new Job { CompanyName = "Test Company 4", Description = "Test Description 4", TimeAtJob = "3 Years", Salary = 5000, Title = "Worker"},
                new Job { CompanyName = "Test Company 5", Description = "Test Description 5", TimeAtJob = "7 Months", Salary = 5000, Title = "Worker"},
            ]
        };

        PdfDocument document = TemplateRenderer.RenderDocumentFromString("PersonReport.html", "PersonReport", person);
        File.WriteAllBytes(document.FileName, document.PDF);
    }    
}