using PDFTemplateLibrary;

namespace TestingConsoleApp;

internal static class Program {
    private static void Main() {
        Person cameron = new(1, "Cameron", "Stocks", 22) {
            Numbers = [12, 1534, 77, 93],
            Children = [
                new() {  Id = 1, Name = "Greg", Surname = "Van Royen", Age = 31, NickName = "Gregory" },
                new() {  Id = 1, Name = "Nicolaas", Surname = "Kruger", Age = 22, NickName = "Techy" },
                new() {  Id = 1, Name = "Harry", Surname = "Vermeulen", Age = 23, NickName = "Harras" },
                new() {  Id = 1, Name = "Michael", Surname = "Yanioglou", Age = 22, NickName = "Miguel" },
            ]
        };
        PdfDocument document = TemplateRenderer.RenderDocument("Person/Person.html", "Person/PersonPDF", cameron);
        File.WriteAllBytes(document.FileFullName, document.PDF);
        
        
        ProjectReport report = new()
        {
            Title = "Website Redesign",
            Manager = "Ava Johnson",
            DateCreated = DateTime.Now,
            Status = "In Progress",
            TeamMembers = new List<string> { "Ava Johnson", "Liam Smith", "Emma Davis", "Noah Lee" },
            Tasks = new List<TaskItem>
            {
                new TaskItem { Title = "Create wireframes", AssignedTo = "Emma Davis", IsCompleted = true },
                new TaskItem { Title = "Build landing page", AssignedTo = "Noah Lee", IsCompleted = false },
                new TaskItem { Title = "Write content", AssignedTo = "Liam Smith", IsCompleted = false },
                new TaskItem { Title = "Client review", AssignedTo = "Ava Johnson", IsCompleted = false },
            }
        };

        document = TemplateRenderer.RenderDocument("ProjectReport/ProjectReport.html", "ProjectReport/ProjectReportPDF", report);
        File.WriteAllBytes(document.FileFullName, document.PDF);
    }
}