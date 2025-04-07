using System.Text.RegularExpressions;
using PDFTemplateLibrary.Helpers;

namespace PDFTemplateLibrary.Iterators;

public class IterationHelper {
    public static IterationMember GetIterationItem(string line) {
        if (!(line.StartsWith(PDFCheck.PDF_FOR_OPEN_TAG)  || !(line.StartsWith(PDFCheck.PDF_FOREACH_OPEN_TAG))) && !line.EndsWith('>')) {
            throw new FormatException("pdf:for tag malformed.");
        }

        IterationMember iterationItem;
        line = line.Replace(">", "").Trim();
        if (line.StartsWith(PDFCheck.PDF_FOREACH_OPEN_TAG)) {
            iterationItem = GetIterationPDFForEach(line.Replace(PDFCheck.PDF_FOREACH_OPEN_TAG, ""));
        } else {
            iterationItem = GetIterationPDFFor(line.Replace(PDFCheck.PDF_FOR_OPEN_TAG, ""));
        }
        
        return iterationItem;
    }

    private static IterationMember GetIterationPDFFor(string line) {
        Match startMatch = Regex.Match(line, @"start=\{(.*?)\}");
        Match endMatch = Regex.Match(line, @"end=\{(.*?)\}");
        Match asMatch = Regex.Match(line, @"as=\{(.*?)\}");

        if (!startMatch.Success || !endMatch.Success || !asMatch.Success) {
            throw new FormatException("pdf:for tag missing required attributes.");
        }

        string startValue = startMatch.Groups[1].Value;
        string endValue = endMatch.Groups[1].Value;
        string asValue = asMatch.Groups[1].Value;

        if (!int.TryParse(startValue, out int start)) {
            throw new FormatException($"Start value '{startValue}' is not a constant integer.");
        }

        int.TryParse(endValue, out int endInt);

        return new IterationMember {
            Start = start,
            End = endInt == 0 ? -1 : endInt,
            Current = start,
            As = asValue,
            EndEvaluator = endValue
        };
    }

    private static IterationMember GetIterationPDFForEach(string line) {
        Match collectionMatch = Regex.Match(line, @"collection=\{(.*?)\}");
        Match asMatch = Regex.Match(line, @"as=\{(.*?)\}");
        
        if (!collectionMatch.Success || !asMatch.Success) {
            throw new FormatException("pdf:foreach tag missing required attributes.");
        }

        IterationMember iterationItem = new() {
            Start = 0,
            End = 0,
            Current = 0,
            As = $"{asMatch.Groups[1].Value}||{collectionMatch.Groups[1].Value}",
            EndEvaluator = $"{collectionMatch.Groups[1].Value}.Count"
        };

        return iterationItem;
    }
}