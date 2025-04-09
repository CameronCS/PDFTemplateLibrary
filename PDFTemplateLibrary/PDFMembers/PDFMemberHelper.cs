using System.Reflection;
using PDFTemplateLibrary.Attributes;

namespace PDFTemplateLibrary.PDFMembers;

public class PDFMemberHelper {
    public static void GetPDFMemberTypeFromProperty(ref object sender, Type dataClass, ref Dictionary<string, PDFMemberType> objectReference, string parentName) {
        PropertyInfo[] dataClassProperties = dataClass.GetProperties();
        foreach (PropertyInfo property in dataClassProperties) {
            bool ignoreFlag = property.GetCustomAttribute<PDFIgnore>() != null;
            if (ignoreFlag) {
                continue;
            }

            string memberName = property.Name;
            Type memberType = property.PropertyType;
            string key = parentName + "." + memberName;
            object memberValue = property.GetValue(sender) ?? $"{key} is null";
            if (IsPrimitiveType(memberType)) {
                objectReference[$"{key}".ToLower()] = new(memberName, memberType, memberValue);
            } else {
                object? nestedInstance = property.GetValue(sender);
                if (nestedInstance != null) {
                    if (typeof(System.Collections.IEnumerable).IsAssignableFrom(memberType) && memberType != typeof(string)) {
                        int index = 0;
                        foreach (object? item in (System.Collections.IEnumerable)nestedInstance) {
                            if (item != null) {
                                string itemKey = $"{key}[{index}]".ToLower();
                                Type itemType = item.GetType();
                                object itemRef = item;
                                if (IsPrimitiveType(itemType)) {
                                    objectReference[itemKey] = new(itemKey, itemType, itemRef) { MemberTypes = null };
                                } else {
                                    GetPDFMemberTypeFromProperty(ref itemRef, itemType, ref objectReference, itemKey);
                                    GetPDFMemberTypeFromMethod(ref itemRef, itemType, ref objectReference, itemKey);
                                }
                                index++;
                            }
                        }

                        objectReference[$"{key}.Count".ToLower()] = new("Count", typeof(int), index);
                    } else {
                        GetPDFMemberTypeFromProperty(ref nestedInstance, memberType, ref objectReference, key);
                    }
                }
            }
        }
    }

    public static void GetPDFMemberTypeFromMethod(ref object sender, Type dataClass, ref Dictionary<string, PDFMemberType> objectReference, string parentName) {
        MethodInfo[] dataClassProperties = dataClass.GetMethods();
        foreach (MethodInfo method in dataClassProperties) {
            bool callFlag = method.GetCustomAttribute<PDFCall>() != null;
            if (!callFlag) {
                continue;
            }

            string memberName = method.Name;
            Type memberType = method.ReturnType;
            string key = $"{parentName}.{memberName}()";
            object memberValue = method.Invoke(sender, []) ?? $"{key} is null";
            if (IsPrimitiveType(memberType)) {
                objectReference[$"{key}".ToLower()] = new(memberName, memberType, memberValue);
            } else {
                GetPDFMemberTypeFromProperty(ref sender, memberType, ref objectReference, key);
            }
        }
    }

    private static bool IsPrimitiveType(Type MemberType) {
        HashSet<Type> types = [
            typeof(bool), typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
            typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(string),
            typeof(Boolean), typeof(Byte), typeof(SByte), typeof(Int16), typeof(UInt16), typeof(Int32), typeof(UInt32),
            typeof(Int64), typeof(UInt64), typeof(Single), typeof(Double), typeof(Decimal), typeof(String),
            typeof(DateTime)
        ];
        return types.Contains(MemberType);
    }
}