using PDFTemplateLibrary.PDFMembers;
using PDFTemplateLibrary.Enums;
using PDFTemplateLibrary.Enums.Helper;

namespace PDFTemplateLibrary.Helpers {
    internal static class PDFHelper {
        public const int LEFT_LINE_SECTION = 0;
        public const int VALUE_LINE_SECTION = 1;
        public const int RIGHT_LINE_SECTION = 2;

        public static string[] GetLineSections(string line) {
            string[] initialSplit = line.Split("{{");
            string leftHandSide = initialSplit[0];
            string[] leftOver = initialSplit[1].Split("}}");
            string templateValue = leftOver[0].Trim();
            string rightHandSide = leftOver[1];
            return [leftHandSide, templateValue, rightHandSide];
        }

        public static string[] GetIfBody(int startIndex, int endIndex, ref string[] lines) {
            List<string> ifBody = [];
            for (int currentIndex = (startIndex + 1); currentIndex < endIndex; currentIndex++) {
                ifBody.Add(lines[currentIndex]);
            }
            return [.. ifBody];
        }

        public static int[] GetConditionIndices(string[] ifBody) {
            List<int> indices = [];
            for (int i = 0; i < ifBody.Length; i++) {
                string trimline = ifBody[i].Trim();
                if (trimline.StartsWith('%') && trimline.EndsWith('%')) {
                    indices.Add(i);
                }
            }

            if (indices.Count == 0) {
                throw new Exception("No conditions found in the if statement.");
            }

            return [.. indices];
        }

        public static bool EvaluateCondition(string condition, ref Dictionary<string, PDFMemberType> objectReference) {
            condition = condition.Trim();
            if (!(condition.StartsWith('%') && condition.EndsWith('%'))) {
                throw new FormatException("If check condition incorrect! Condition must start and end with a '%'");
            }
            string conditionBody = condition[1..^1].Trim();
            ConditionOperator conditionOperator = ConditionHelper.GetConditionOperator(conditionBody);
            string[] ifConditionSections = ConditionHelper.GetConditionSections(conditionBody, conditionOperator);
            PDFMemberType member = objectReference[ifConditionSections[0].Trim().ToLower()];
            return ConditionHelper.EvaluateCondition(member, conditionOperator, ifConditionSections[1].Trim());
        }
    }
}
