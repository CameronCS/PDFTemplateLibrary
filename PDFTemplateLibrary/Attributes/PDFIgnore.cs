using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTemplateLibrary.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PDFIgnore : Attribute {
    }
}
