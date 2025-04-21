namespace PDFTemplateLibrary;

public static class TemplateRenderer {
    public static PdfDocument RenderDocumentFromFile(string templatePath, string filename, object DataObject) {
        PdfDocumentGenerator PDFRenderer = new(templatePath, filename);
        PDFRenderer.CreatePDFObject(DataObject);
        PdfDocument document = PDFRenderer.GenerateDocument();
        return document;
    }
    public static PdfDocument RenderDocumentFromString(string templateString, string filename, object DataObject) {
        string[] templateLines = templateString.Split("\r\n");
        PdfDocumentGenerator PDFRenderer = new(templateLines, filename);
        PDFRenderer.CreatePDFObject(DataObject);
        PdfDocument document = PDFRenderer.GenerateDocument();
        return document;
    }
}