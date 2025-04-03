namespace PDFTemplateLibrary;

public class TemplateRenderer {
    public static PDFDocument RenderDocument(string templatePath, string filename, object DataObject) {
        PDFDocumentGenerator PDFRenderer = new(templatePath, filename);
        PDFRenderer.CreatePDFObject(DataObject);
        PDFDocument document = PDFRenderer.GenerateDocument();
        return document;
    }
}
