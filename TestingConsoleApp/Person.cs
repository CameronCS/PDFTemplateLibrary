using PDFTemplateLibrary.Attributes;

namespace TestingConsoleApp {
    internal class Person(int id, string name, string surname, int age) {
        [PDFIgnore] public int Id { get; set; } = id;
        public string UID => $"{name[0]}{name[^1]}{Id}{age}{surname[0]}{surname[^1]}";
        public string Name { get; set; } = name;
        public string Surname { get; set; } = surname;
        public int Age { get; set; } = age;

        [PDFCall] public string GetFullName() {
            return this.Name + " " + this.Surname;
        }
    }
}
