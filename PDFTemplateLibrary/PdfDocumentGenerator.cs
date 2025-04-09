using System.Diagnostics;
using System.Text;
using PDFTemplateLibrary.PDFMembers;
using PDFTemplateLibrary.Helpers;
using PDFTemplateLibrary.Iterators;
using PDFTemplateLibrary.PDFRules;

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
            string line = this._templateLines[currentLineNumber].Trim();
            if (line.StartsWith(PDFCheck.PDF_RULE_MODEL_NAME)) {
                ModelNameRule modelNameRule = ModelRuleHelper.GetModelNameRule(line);
                Dictionary<string, PDFMemberType> temp = [];
                foreach (KeyValuePair<string, PDFMemberType> objectReference in this._objectReference) {
                    if (objectReference.Key.Contains(modelNameRule.ModelName.ToLower())) {
                        temp[objectReference.Key.Replace(modelNameRule.ModelName.ToLower(), modelNameRule.ModelAs.ToLower())] = objectReference.Value;
                    } else {
                        temp[objectReference.Key] = objectReference.Value;
                    }
                }
                this._objectReference = temp;
            }
            
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
                int forStartIndex = currentLineNumber;
                int forEndIndex = this.FindEndIndex(forStartIndex, PDFCheck.PDF_FOREACH_CLOSE_TAG);
                IterationMember iteration = IterationHelper.GetIterationItem(line.Trim());
                pdfLines.AddRange(this.HandlePDFForEach(forStartIndex, forEndIndex, iteration));
                currentLineNumber = forEndIndex;
            }
            if (this.IsPDFEndFOrEach(line)) continue;



            if (this.IsPDFFor(line)) {
                IterationMember iteration = IterationHelper.GetIterationItem(line.Trim());
                if (iteration.End == -1) iteration.End = (int)this._objectReference[iteration.EndEvaluator.ToLower()].Value;
                int forStartIndex = currentLineNumber;
                int forEndIndex = this.FindEndIndex(forStartIndex, PDFCheck.PDF_FOR_CLOSE_TAG);
                pdfLines.AddRange(this.HandlePDFFor(forStartIndex, forEndIndex, iteration));
                currentLineNumber = forEndIndex;
            }
            if (this.IsPDFEndFor(line)) continue;

            pdfLines.Add(this.IsTemplateLine(line) ? this.RenderLine(line) : line);
        }

        this._templateLines = [.. pdfLines];
    }

    private bool IsPdfIF(string lineCheck) => lineCheck.Contains(PDFCheck.PDF_IF_OPEN_TAG);
    private bool IsPdfEndIF(string lineCheck) => lineCheck.Contains(PDFCheck.PDF_IF_CLOSE_TAG);

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

    private string[] HandlePDFFor(int startIndex, int endIndex, IterationMember iterationMember) {
        List<string> forLines = [];
        while (iterationMember.Current != iterationMember.End) {
            this._objectReference[iterationMember.As] = new(iterationMember.As, typeof(int), iterationMember.Current);
            string forLine = "";
            for (int forIndex = startIndex + 1; forIndex < endIndex; forIndex++) {
                forLine = this._templateLines[forIndex];
                if (forLine.Contains('[') && forLine.Contains(']')) {
                    forLine = forLine.Replace(iterationMember.As, $"{iterationMember.Current}");
                }

                forLines.Add(this.IsTemplateLine(forLine) ? this.RenderLine(forLine) : forLine);
            }

            iterationMember.Current++;
        }

        this._objectReference.Remove(iterationMember.As);
        
        return [.. forLines];
    }
    
    private bool IsPDFForEach(string line) => line.Trim().StartsWith(PDFCheck.PDF_FOREACH_OPEN_TAG);
    private bool IsPDFEndFOrEach(string line) => line.Trim().StartsWith(PDFCheck.PDF_FOREACH_OPEN_TAG);
    
        private string[] HandlePDFForEach(int startIndex, int EndIndex, IterationMember iterationMember) {
            List<string> foreachLines = [];
    
            
            string[] forEachMembers = iterationMember.As.Split("||");
            string collectionName = forEachMembers[0];
            string itemName = forEachMembers[1];
            iterationMember.End = (int)this._objectReference[iterationMember.EndEvaluator.ToLower()].Value;
            while (iterationMember.Current != iterationMember.End) {
                string forLine = "";
                for (int forIndex = startIndex + 1; forIndex < EndIndex; forIndex++) {
                    forLine = this._templateLines[forIndex];
                    if (this.IsTemplateLine(forLine)) {
                        if (forLine.Contains(collectionName)) {
                            forLine = forLine.Replace(collectionName, $"{itemName}[{iterationMember.Current}]");
                        }
    
                        foreachLines.Add(this.RenderLine(forLine));
                    } else {
                        foreachLines.Add(forLine);
                    }
                }
    
                iterationMember.Current++;
            }
            return [.. foreachLines];
        }

    private int FindEndIndex(int startIndex, string closeTag) {
        for (int i = startIndex; i < this._templateLines.Length; i++) {
            string line = this._templateLines[i];
            if (line.Contains(closeTag)) {
                return i;
            }
        }

        throw new Exception("Statement body never terminated");
    }

    private int SearchForClosingBrace(int startIndex, string[] ifBody) {
        for (int i = startIndex; i < ifBody.Length; i++) {
            if (ifBody[i].Contains('}') && !ifBody[i].Contains("}}")) {
                return i;
            }
        }

        throw new Exception("If statement body never terminated");
    }


    public PdfDocument GenerateDocument()
    {
        string pathToWkHtmlToPDF = "./wkhtmltopdf/wkhtmltopdf.exe";
        string htmlContent = string.Join(Environment.NewLine, this._templateLines);

        ProcessStartInfo processStartInfo = new()
        {
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
        process.StandardInput.AutoFlush = true;

        // Write HTML to stdin and close the stream
        using (StreamWriter writer = new(process.StandardInput.BaseStream, Encoding.UTF8))
        {
            writer.Write(htmlContent);
        }

        using MemoryStream memoryStream = new();
        process.StandardOutput.BaseStream.CopyTo(memoryStream);

        string errorOutput = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"PDF Creation exited with code {process.ExitCode}\n{errorOutput}");
        }

        this._data = memoryStream.ToArray();
        return new PdfDocument(this._fileName, this._data);
    }
}