using PDFTemplateLibrary.Attributes;

namespace TestingConsoleApp {
    internal class Person(int id, string name, string surname, int age) {
        [PDFIgnore] private int Id { get; set; } = id;
        public string UID => $"{name[0]}{name[^1]}{Id}{age}{surname[0]}{surname[^1]}";
        public string Name { get; set; } = name;
        public string Surname { get; set; } = surname;
        public int Age { get; set; } = age;
        public List<int> Numbers { get; set; } = [];
        public List<Child> Children { get; set; } = []; 
        
        [PDFCall] public string GetFullName() {
            return this.ConcatName();
        }

        public string ConcatName() {
            return this.Name + " " + this.Surname;
        }
    }
}
