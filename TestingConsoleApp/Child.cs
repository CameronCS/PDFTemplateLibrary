using PDFTemplateLibrary.Attributes;

namespace TestingConsoleApp;

public class Child {
    [PDFIgnore] public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public int Age { get; set; }
    public string NickName { get; set; }
}