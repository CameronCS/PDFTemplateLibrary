using PDFTemplateLibrary.PDFMembers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFTemplateLibrary.Enums.Helper {
    internal class ConditionHelper {
        public static ConditionOperator GetConditionOperator(string line) {


            if (line.Contains(">="))
                return ConditionOperator.GreaterThanOrEqual;

            if (line.Contains("<="))
                return ConditionOperator.LessThanOrEqual;

            if (line.Contains("!="))
                return ConditionOperator.NotEqual;
            
            if (line.Contains('='))
                return ConditionOperator.Equal;

            if (line.Contains('>'))
                return ConditionOperator.GreaterThan;
            
            if (line.Contains('<'))
                return ConditionOperator.LessThan;

            throw new FormatException("Invalid condition operator in line: " + line);
        }

        public static string GetConditionOperatorString(ConditionOperator conditionOperator) {
            return conditionOperator switch {
                ConditionOperator.Equal => "=",
                ConditionOperator.GreaterThan => ">",
                ConditionOperator.LessThan => "<",
                ConditionOperator.GreaterThanOrEqual => ">=",
                ConditionOperator.LessThanOrEqual => "<=",
                ConditionOperator.NotEqual => "!=",
                _ => throw new ArgumentOutOfRangeException(nameof(conditionOperator), conditionOperator, null)
            };
        }

        public static string[] GetConditionSections(string line, ConditionOperator conditionOperator) {
            string operatorString = GetConditionOperatorString(conditionOperator);
            string[] sections = line.Split(operatorString);
            return sections;
        }

        public static bool EvaluateCondition(PDFMemberType member, ConditionOperator conditionOperator, string compareValue) {
            if (member.Type == typeof(int)) {
                int memberValue = (int)member.Value;
                bool isInt = int.TryParse(compareValue, out int compareIntValue);
                if (!isInt) throw new ArgumentException($"Comparison on {member.Name} for value {compareValue} does not match type int");
                
                return conditionOperator switch {
                    ConditionOperator.Equal => memberValue == compareIntValue,
                    ConditionOperator.GreaterThan => memberValue > compareIntValue,
                    ConditionOperator.LessThan => memberValue < compareIntValue,
                    ConditionOperator.GreaterThanOrEqual => memberValue >= compareIntValue,
                    ConditionOperator.LessThanOrEqual => memberValue <= compareIntValue,
                    ConditionOperator.NotEqual => memberValue != compareIntValue,
                    _ => throw new ArgumentOutOfRangeException(nameof(conditionOperator), conditionOperator, null)
                };
            }

            if (member.Type == typeof(string)) {
                return conditionOperator switch {
                    ConditionOperator.Equal => member.Value == compareValue,
                    ConditionOperator.NotEqual => member.Value != compareValue,
                    _ => throw new ArgumentException("Invalid Comparison on a string value")
                };
            }

            if (member.Type == typeof(double)) {
                double memberValue = (double)member.Value;
                bool isDouble = double.TryParse(compareValue, out double compareDoubleValue);
                if (!isDouble)
                    throw new ArgumentException($"Comparison on {member.Name} for value {compareValue} does not match type int");

                return conditionOperator switch {
                    ConditionOperator.Equal => memberValue == compareDoubleValue,
                    ConditionOperator.GreaterThan => memberValue > compareDoubleValue,
                    ConditionOperator.LessThan => memberValue < compareDoubleValue,
                    ConditionOperator.GreaterThanOrEqual => memberValue >= compareDoubleValue,
                    ConditionOperator.LessThanOrEqual => memberValue <= compareDoubleValue,
                    ConditionOperator.NotEqual => memberValue != compareDoubleValue,
                    _ => throw new ArgumentOutOfRangeException(nameof(conditionOperator), conditionOperator, null)
                };
            }

            if (member.Type == typeof(bool)) {
                bool memberValue = (bool)member.Value;
                bool compareBoolValue = bool.Parse(compareValue);
                return conditionOperator switch {
                    ConditionOperator.Equal => memberValue == compareBoolValue,
                    ConditionOperator.NotEqual => memberValue != compareBoolValue,
                    _ => throw new ArgumentException("Invalid Comparison on a bool value")
                };
            }

            throw new NotSupportedException("Unsupported member type: " + member.Type);
        }
    }
}
