﻿using PDFTemplateLibrary;

namespace TestingConsoleApp;

internal static class Program {
    private static void Main() {
        Person cameron = new(1, "Cameron", "Stocks", 22) {
            Numbers = [12, 1534, 77, 93],
            Children = [
                new() {  Id = 1, Name = "Greg", Surname = "Van Royen", Age = 31 },
                new() {  Id = 1, Name = "Nicolaas", Surname = "Kruger", Age = 22 },
                new() {  Id = 1, Name = "Harry", Surname = "Vermeulen", Age = 23 },
                new() {  Id = 1, Name = "Miguel", Surname = "Yanioglou", Age = 22 }
            ]
        };
        PdfDocument document = TemplateRenderer.RenderDocument("Person.html", "PersonPDF", cameron);
        File.WriteAllBytes(document.FileFullName, document.PDF);
    }
}