using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTemplateLibrary {
    public class PDFDocument(string fileName, byte[] pdfData) {
        public string FileName { get; } = fileName;
        public string FileFullName { get; } = $"{fileName}.html";
        public byte[] PDF { get; } = pdfData;
    }
}
