using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PDFTemplateLibrary.ClassMembers {
    internal class PDFMemberType(string name, Type type, dynamic value) {
        public string Name { get; set; } = name;

        public Type Type { get; set; } = type;

        public dynamic Value { get; set; } = value;
    }
}