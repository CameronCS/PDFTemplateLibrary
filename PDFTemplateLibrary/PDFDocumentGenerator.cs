using PDFTemplateLibrary.Attributes;
using PDFTemplateLibrary.ClassMembers;
using PDFTemplateLibrary.Helpers;
using System.ComponentModel;
using System.Reflection;

namespace PDFTemplateLibrary;

public class PDFDocumentGenerator(string templatePath, string fileName) {
    private string _fileName = fileName;
    private object _dataClass = new();

    private string _className = string.Empty;

    public string[] _templateLines = File.ReadAllLines(templatePath);
    private byte[] _data = [];

    private Dictionary<string, PDFMemberType> _objectReference = [];

    public void CreatePDFObject(object PDFDataClass) {
        ArgumentNullException.ThrowIfNull(PDFDataClass);
        this._dataClass = PDFDataClass;
        Type objectType = PDFDataClass.GetType();
        this._className = this.GetTypeName(objectType);

        this.GetFieldMembers(objectType);

        this.RenderPDFLines();
        File.WriteAllLines("FinishedPersonPDF.html", this._templateLines);
    }

    private string GetTypeName(Type type) => type.Name.Split('.')[^1];

    private void GetFieldMembers(Type DataClassType) {
        this.ExtractProperties(DataClassType);
        this.ExtractMethods(DataClassType);
    }

    private void ExtractProperties(Type DataClassType) {
        PropertyInfo[] DataClassProperties = DataClassType.GetProperties();
        foreach (PropertyInfo property in DataClassProperties) {
            bool ignoreFlag = property.GetCustomAttribute<PDFIgnore>() != null;
            if (!ignoreFlag) {
                string memberName = property.Name;
                Type memberType = property.PropertyType;
                object memberValue = property.GetValue(this._dataClass) ?? $"{this._className}.{memberName} is null";
                this._objectReference[$"{this._className}.{memberName}".ToLower()] = new PDFMemberType(memberName, memberType, memberValue);
            }
        }
    }

    private void ExtractMethods(Type DataClassType) {
        MethodInfo[] DataClassMethods = DataClassType.GetMethods();
        List<MethodInfo> callableMethods = new();
        foreach (MethodInfo method in DataClassMethods) {
            bool callFlag = method.GetCustomAttribute<PDFCall>() != null;
            if (callFlag) {
                string memberName = method.Name;
                Type memberType = method.ReturnType;
                object memberValue = method.Invoke(this._dataClass, []) ?? $"{this._className}.{memberName}() retuned null";
                this._objectReference[$"{this._className}.{memberName}()".ToLower()] = new PDFMemberType(memberName, memberType, memberValue);
            }
        }

    }

    private void RenderPDFLines() {
        List<string> pdfLines = [];
        for (int i = 0; i < this._templateLines.Length; i++) {
            string line = this._templateLines[i];
            if (this.IsPdfIF(line)) {
                int ifStartIndex = i;
                int ifEndIndex = this.FindIfEndIndex(i);
                string[] ifBody = PDFHelper.GetIfBody(ifStartIndex, ifEndIndex, ref this._templateLines);
                string[] ifReturns = this.HandlePDFIf(ifBody);
                foreach (string ifReturnLine in ifReturns) {
                    pdfLines.Add(this.IsTemplateLine(ifReturnLine) ? this.RenderLine(ifReturnLine) : ifReturnLine);
                }
                i = ifEndIndex;
                continue;
            }

            if (IsPdfEndIF(line)) continue;

            pdfLines.Add(this.IsTemplateLine(line) ? this.RenderLine(line) : line);
        }
        this._templateLines = [.. pdfLines];
    }

    private bool IsPdfIF(string lineCheck) => lineCheck.Contains(PDFCheck.PDF_IF_OPEN_TAG);
    private bool IsPdfEndIF(string lineCheck) => lineCheck.Contains(PDFCheck.PDF_IF_CLOSE_TAG);

    private int FindIfEndIndex(int startIndex) {
        for (int i = startIndex; i < this._templateLines.Length; i++) {
            string line = this._templateLines[i];
            if (line.Contains(PDFCheck.PDF_IF_CLOSE_TAG)) {
                return i;
            }
        }
        throw new Exception("If statement body never terminated");
    }

    private string[] HandlePDFIf(string[] ifBody) {
        List<string> returnLines = [];
        int[] conditionIndices = PDFHelper.GetConditionIndicies(ifBody);
        foreach (int index in conditionIndices) {
            if (index == conditionIndices[^1]) { // This is the default case
                int openBraceIndex = ifBody[index].Contains('{') && !ifBody[index].Contains("{{") ? index : index + 1;
                int closingBraceIndex = this.SearchForClosingBrace(openBraceIndex, ifBody);
                returnLines.AddRange(this.GetIfReturnLines(ifBody, openBraceIndex, closingBraceIndex));
                return [.. returnLines];
            }

            bool statementValid = PDFHelper.EvaluateCondition(ifBody[index], ref this._objectReference);
            if (statementValid) {
                int openBraceIndex = ifBody[index].Contains('{') && !ifBody[index].Contains("{{") ? index : index + 1;
                int closingBraceIndex = this.SearchForClosingBrace(openBraceIndex, ifBody);
                returnLines.AddRange(this.GetIfReturnLines(ifBody, openBraceIndex, closingBraceIndex));
                return [.. returnLines];
            }
        }
        return [..returnLines];
    }

    private int SearchForClosingBrace(int startIndex, string[] ifBody) {
        for (int i = startIndex; i < ifBody.Length; i++) {
            if (ifBody[i].Contains('}') && !ifBody[i].Contains("}}")) {
                return i;
            }
        }
        throw new Exception("If statement body never terminated");
    }

    private string[] GetIfReturnLines(string[] ifBody, int startIndex, int endIndex) {
        List<string> returnLines = new();
        for (int currentIndex = startIndex + 1; currentIndex < endIndex; currentIndex++) {
            returnLines.Add(ifBody[currentIndex]);
        }
        return returnLines.ToArray();
    }


    private bool IsTemplateLine(string line) => line.Contains("{{") && line.Contains("}}");
    private string RenderLine(string line) {
        string[] lineSections = PDFHelper.GetLineSections(line);
        PDFMemberType memberType = _objectReference[lineSections[PDFHelper.VALUE_LINE_SECTION].ToLower()];
        string templateLine = $"{lineSections[PDFHelper.LEFT_LINE_SECTION]}{memberType.Value}{lineSections[PDFHelper.RIGHT_LINE_SECTION]}";
        return templateLine;
    }

    public PDFDocument GenerateDocument() {
        return new PDFDocument(_fileName, _data);
    }
}
