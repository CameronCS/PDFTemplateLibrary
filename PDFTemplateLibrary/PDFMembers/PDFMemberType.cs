namespace PDFTemplateLibrary.PDFMembers {
    public class PDFMemberType(string name, Type type, dynamic value) {
        public string Name { get; set; } = name;

        public Type Type { get; set; } = type;

        public dynamic Value { get; set; } = value;
        
        public PDFMemberType[]? MemberTypes { get; set; }
    }
}