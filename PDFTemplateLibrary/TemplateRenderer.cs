namespace PDFTemplateLibrary;

public class TemplateRenderer {
    public static PdfDocument RenderDocument(string templatePath, string filename, object DataObject) {
        PdfDocumentGenerator PDFRenderer = new(templatePath, filename);
        PDFRenderer.CreatePDFObject(DataObject);
        PdfDocument document = PDFRenderer.GenerateDocument();
        return document;
    }
}
