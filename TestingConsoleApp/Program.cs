using PDFTemplateLibrary;

namespace TestingConsoleApp;

internal class Program {
    private static void Main(string[] args) {
        Person Cameron = new(1, "Cameron", "Stocks", 21);
        PDFDocument document = TemplateRenderer.RenderDocument("Person.html", "PersonPDF", Cameron);

        File.WriteAllBytes(document.FileFullName, document.PDF);
    }
}