using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTemplateLibrary.Helpers {
    internal class PDFCheck {
        public const string PDF_IF_OPEN_TAG = "<pdf:if>";
        public const string PDF_IF_CLOSE_TAG = "</pdf:if>";
        
        public const string PDF_FOR_OPEN_TAG = "<pdf:for";
        public const string PDF_FOR_CLOSE_TAG = "</pdf:for>";
        
        public const string PDF_FOREACH_OPEN_TAG = "<pdf:foreach";
        public const string PDF_FOREACH_CLOSE_TAG = "</pdf:foreach>";
    }
}
