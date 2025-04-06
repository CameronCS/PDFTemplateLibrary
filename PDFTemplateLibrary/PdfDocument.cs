namespace PDFTemplateLibrary {
    public class PdfDocument(string fileName, byte[] pdfData) {
        public string FileName { get; } = fileName;
        public string FileFullName { get; } = $"{fileName}.pdf";
        public byte[] PDF { get; } = pdfData;
    }
}
