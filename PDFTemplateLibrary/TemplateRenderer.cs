namespace PDFTemplateLibrary;

public class TemplateRenderer {
    public static PdfDocument RenderDocumentFromFile(string templatePath, string filename, object DataObject) {
        PdfDocumentGenerator PDFRenderer = new(templatePath, filename);
        PDFRenderer.CreatePDFObject(DataObject);
        PdfDocument document = PDFRenderer.GenerateDocument();
        return document;
    }
    public static PdfDocument RenderDocumentFromString(string templatePath, string filename, object DataObject) {
        PdfDocumentGenerator PDFRenderer = new(templatePath, filename);
        PDFRenderer.CreatePDFObject(DataObject);
        PdfDocument document = PDFRenderer.GenerateDocument();
        return document;
    }
}
