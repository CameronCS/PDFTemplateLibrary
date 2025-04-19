using System.Text.RegularExpressions;

namespace PDFTemplateLibrary.PDFRules;

public static class ModelRuleHelper {
    public static ModelNameRule GetModelNameRule(string line) {
        Match modelMatch = Regex.Match(line, @"modelname=\{(.*?)\}");
        Match asMatch = Regex.Match(line, @"as=\{(.*?)\}");

        if (!modelMatch.Success && !asMatch.Success) {
            throw new ArgumentException("Invalid model rule line");
        }

        return new() {
            ModelName = modelMatch.Groups[1].Value,
            ModelAs = asMatch.Groups[1].Value
        };
    }
}