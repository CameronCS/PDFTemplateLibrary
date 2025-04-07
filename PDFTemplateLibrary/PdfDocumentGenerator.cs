using System.Diagnostics;
using System.Text;
using PDFTemplateLibrary.PDFMembers;
using PDFTemplateLibrary.Helpers;
using PDFTemplateLibrary.Iterators;

namespace PDFTemplateLibrary;

public class PdfDocumentGenerator(string templatePath, string fileName) {
    private readonly string _fileName = fileName;
    private object _dataClass = new();
    
    public string[] _templateLines = File.ReadAllLines(templatePath);
    private byte[] _data = [];

    private Dictionary<string, PDFMemberType> _objectReference = [];

    public void CreatePDFObject(object pdfDataClass) {
        ArgumentNullException.ThrowIfNull(pdfDataClass);
        this._dataClass = pdfDataClass;
        Type objectType = pdfDataClass.GetType();

        this.GetFieldMembers(objectType);

        this.RenderPDFLines();
        File.WriteAllLines("FinishedPersonPDF.html", this._templateLines);
    }
    
    private void GetFieldMembers(Type dataClassType) {
        string className = dataClassType.Name;
        PDFMemberHelper.GetPDFMemberTypeFromProperty(ref this._dataClass, dataClassType, ref this._objectReference, className);
        PDFMemberHelper.GetPDFMemberTypeFromMethod(ref this._dataClass, dataClassType, ref this._objectReference, className);
    }
    
    private void RenderPDFLines() {
        List<string> pdfLines = [];
        for (int currentLineNumber = 0; currentLineNumber < this._templateLines.Length; currentLineNumber++) {
            string line = this._templateLines[currentLineNumber];
            if (this.IsPdfIF(line)) {
                int ifStartIndex = currentLineNumber;
                int ifEndIndex = this.FindEndIndex(currentLineNumber, PDFCheck.PDF_IF_CLOSE_TAG);
                string[] ifBody = PDFHelper.GetIfBody(ifStartIndex, ifEndIndex, ref this._templateLines);
                string[] ifReturns = this.HandlePDFIf(ifBody);
                foreach (string ifReturnLine in ifReturns) {
                    pdfLines.Add(this.IsTemplateLine(ifReturnLine) ? this.RenderLine(ifReturnLine) : ifReturnLine);
                }
                currentLineNumber = ifEndIndex;
                continue;
            }
            
            if (this.IsPdfEndIF(line)) continue;

            if (this.IsPDFForEach(line)) {
                IterationMember iteration = IterationHelper.GetIterationItem(line.Trim());
                string[] forEachMembers = iteration.As.Split("||");
                string collectionName = forEachMembers[0];
                string itemName = forEachMembers[1];
                iteration.End = (int)this._objectReference[iteration.EndEvaluator].Value;
                int forStartIndex = currentLineNumber;
                int forEndIndex = this.FindEndIndex(forStartIndex, PDFCheck.PDF_FOREACH_CLOSE_TAG);
                while (iteration.Current != iteration.End) {
                    string forLine = "";
                    for (int forIndex = forStartIndex + 1; forIndex < forEndIndex; forIndex++) {
                        forLine = this._templateLines[forIndex];
                        if (this.IsTemplateLine(forLine)) {
                            if (forLine.Contains(collectionName)) {
                                forLine = forLine.Replace(collectionName, $"{itemName}[{iteration.Current}]");
                            }
                            pdfLines.Add(this.RenderLine(forLine));
                        } else {
                            pdfLines.Add(forLine);
                        }
                    }

                    iteration.Current++;
                }
                currentLineNumber = forEndIndex;
            }
            
            if (this.IsPDFForEach(line)) continue;

            
            if (this.IsPDFFor(line)) {
                IterationMember iteration = IterationHelper.GetIterationItem(line.Trim());
                if (iteration.End == -1)
                    iteration.End = (int)this._objectReference[$"{iteration.EndEvaluator}"].Value;

                int forStartIndex = currentLineNumber;
                int forEndIndex = this.FindEndIndex(forStartIndex, PDFCheck.PDF_FOR_CLOSE_TAG);
                while (iteration.Current != iteration.End) {
                    this._objectReference[iteration.As] = new(iteration.As, typeof(int), iteration.Current);
                    string forLine = "";
                    for (int forIndex = forStartIndex + 1; forIndex < forEndIndex; forIndex++) {
                        forLine = this._templateLines[forIndex];
                        if (forLine.Contains('[') && forLine.Contains(']')) {
                            forLine = forLine.Replace(iteration.As, $"{iteration.Current}");
                        }
                        pdfLines.Add(this.IsTemplateLine(forLine) ? this.RenderLine(forLine) : forLine);
                    }
                    iteration.Current++;
                }
                this._objectReference.Remove(iteration.As);
                currentLineNumber = forEndIndex;
            }
            
            if (this.IsPDFEndFor(line)) continue;
            
            pdfLines.Add(this.IsTemplateLine(line) ? this.RenderLine(line) : line);
        }
        this._templateLines = [.. pdfLines];
    }

    private bool IsPdfIF(string lineCheck) => lineCheck.Contains(PDFCheck.PDF_IF_OPEN_TAG);
    private bool IsPdfEndIF(string lineCheck) => lineCheck.Contains(PDFCheck.PDF_IF_CLOSE_TAG);

    private int FindEndIndex(int startIndex, string closeTag) {
        for (int i = startIndex; i < this._templateLines.Length; i++) {
            string line = this._templateLines[i];
            if (line.Contains(closeTag)) {
                return i;
            }
        }
        throw new Exception("Statement body never terminated");
    }

    private string[] HandlePDFIf(string[] ifBody) {
        List<string> returnLines = [];
        int[] conditionIndices = PDFHelper.GetConditionIndices(ifBody);
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
        List<string> returnLines = [];
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

    private bool IsPDFFor(string line) => line.Trim().StartsWith(PDFCheck.PDF_FOR_OPEN_TAG); 
    private bool IsPDFEndFor(string line) => line.Trim().StartsWith(PDFCheck.PDF_FOR_OPEN_TAG); 
    
    private bool IsPDFForEach(string line) => line.Trim().StartsWith(PDFCheck.PDF_FOREACH_OPEN_TAG);
    private bool IsPDFEndFOrEach(string line) => line.Trim().StartsWith(PDFCheck.PDF_FOREACH_OPEN_TAG);
    
    public PdfDocument GenerateDocument() {
        string pathToWkHtmlToPDF = "./wkhtmltopdf/wkhtmltopdf.exe";
        
        string htmlContent = string.Join(Environment.NewLine, this._templateLines);

        ProcessStartInfo processStartInfo = new() {
            FileName = pathToWkHtmlToPDF, 
            Arguments = "-q - -", 
            UseShellExecute = false, 
            RedirectStandardInput = true, 
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using Process process = new();
        process.StartInfo = processStartInfo;
        process.Start();

        {
            using StreamWriter writer = new(process.StandardInput.BaseStream, Encoding.UTF8);
            writer.WriteLine(htmlContent);
        }

        {
            using MemoryStream memoryStream = new();
            process.StandardOutput.BaseStream.CopyTo(memoryStream);
            process.WaitForExit();
            if (process.ExitCode != 0) {
                string error = process.StandardError.ReadToEnd();
                throw new Exception($"PDF Creation exited with a code of: {process.ExitCode}\n{error}");
            }
            this._data = memoryStream.ToArray();
        }
        
        return new PdfDocument(this._fileName, this._data);
    }
}
