using System.Text.RegularExpressions;

namespace PDFTemplateLibrary.Iterators;

public class IterationHelper {
    public static IterationMember GetIterationItem(string line) {
        if (!(line.StartsWith("<pdf:for") && line.EndsWith('>'))) {
            throw new FormatException("pdf:for tag malformed.");
        }

        line = line.Replace("<pdf:for", "").Replace(">", "").Trim();

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
}