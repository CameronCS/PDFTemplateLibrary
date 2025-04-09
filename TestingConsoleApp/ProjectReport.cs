using PDFTemplateLibrary.Attributes;

namespace TestingConsoleApp;

public class ProjectReport {
    public string Title { get; set; }
    public string Manager { get; set; }
    public DateTime DateCreated { get; set; }
    public string Status { get; set; }
    public List<string> TeamMembers { get; set; }
    public List<TaskItem> Tasks { get; set; }

    [PDFCall] public string GetFormattedDate() {
        return DateCreated.ToString("MMMM dd, yyyy");
    }
}

public class TaskItem {
    public string Title { get; set; }
    public string AssignedTo { get; set; }
    public bool IsCompleted { get; set; }

    [PDFCall] public string GetCompleteString() {
        return IsCompleted ? "<span class='completed'>Completed</span>" : "<span class='incomplete'>Incomplete</span>";
    }
}