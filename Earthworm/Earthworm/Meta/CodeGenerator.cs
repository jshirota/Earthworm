using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Geodatabase;

namespace Earthworm.Meta
{
    internal class CodeGenerator
    {
        private readonly string[] _keywords = { "abstract", "add", "addhandler", "addressof", "aggregate", "alias", "and", "andalso", "ansi", "as", "ascending", "assembly", "async", "auto", "await", "base", "binary", "bool", "boolean", "break", "by", "byref", "byte", "byval", "call", "case", "catch", "cbool", "cbyte", "cchar", "cdate", "cdbl", "cdec", "char", "checked", "cint", "class", "clng", "cobj", "compare", "const", "continue", "csbyte", "cshort", "csng", "cstr", "ctype", "cuint", "culng", "cushort", "custom", "date", "decimal", "declare", "default", "delegate", "descending", "dim", "directcast", "distinct", "do", "double", "dynamic", "each", "else", "elseif", "end", "endif", "enum", "equals", "erase", "error", "event", "exit", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "friend", "from", "function", "get", "gettype", "getxmlnamespace", "global", "gosub", "goto", "group", "handles", "if", "implements", "implicit", "imports", "in", "inherits", "int", "integer", "interface", "internal", "into", "is", "isfalse", "isnot", "istrue", "iterator", "join", "key", "let", "lib", "like", "lock", "long", "loop", "me", "mid", "mod", "module", "mustinherit", "mustoverride", "mybase", "myclass", "namespace", "narrowing", "new", "next", "not", "nothing", "notinheritable", "notoverridable", "null", "object", "of", "off", "on", "operator", "option", "optional", "or", "order", "orderby", "orelse", "out", "overloads", "overridable", "override", "overrides", "paramarray", "params", "partial", "preserve", "private", "property", "protected", "public", "raiseevent", "readonly", "redim", "ref", "rem", "remove", "removehandler", "resume", "return", "sbyte", "sealed", "select", "set", "shadows", "shared", "short", "single", "sizeof", "skip", "stackalloc", "static", "step", "stop", "strict", "string", "struct", "structure", "sub", "switch", "synclock", "take", "text", "then", "this", "throw", "to", "true", "try", "trycast", "typeof", "uint", "uinteger", "ulong", "unchecked", "unicode", "unsafe", "until", "ushort", "using", "value", "var", "variant", "virtual", "void", "volatile", "wend", "when", "where", "while", "widening", "with", "withevents", "writeonly", "xor", "yield" };
        private readonly StringBuilder _cs = new StringBuilder();
        private readonly StringBuilder _vb = new StringBuilder();
        private readonly bool _hasNamespace;

        #region Private

        private void AppendUsingStatements()
        {
            _cs.AppendLine("using System;\r\nusing Earthworm;\r\n");
            _vb.AppendLine("Imports Earthworm\r\n");
        }

        private void AppendBeginning(string namespaceName, string className)
        {
            if (_hasNamespace)
            {
                _cs.AppendFormat("namespace {0}\r\n{{\r\n    public class {1} : MappableFeature\r\n    {{", namespaceName, className);
                _vb.AppendFormat("Namespace {0}\r\n\r\n    Public Class {1}\r\n        Inherits MappableFeature\r\n", namespaceName, className);
            }
            else
            {
                _cs.AppendFormat("public class {0} : MappableFeature\r\n{{", className);
                _vb.AppendFormat("Public Class {0}\r\n    Inherits MappableFeature\r\n", className);
            }
        }

        private void AppendProperty(string name, esriFieldType fieldType, int length, bool isNullable)
        {
            string csType;
            string vbType;
            string lengthText = null;

            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeString:
                    csType = "string";
                    vbType = "String";
                    lengthText = ", " + length;
                    break;
                case esriFieldType.esriFieldTypeInteger:
                    csType = "int";
                    vbType = "Integer";
                    break;
                case esriFieldType.esriFieldTypeSmallInteger:
                    csType = "short";
                    vbType = "Short";
                    break;
                case esriFieldType.esriFieldTypeDouble:
                    csType = "double";
                    vbType = "Double";
                    break;
                case esriFieldType.esriFieldTypeSingle:
                    csType = "float";
                    vbType = "Single";
                    break;
                case esriFieldType.esriFieldTypeDate:
                    csType = "DateTime";
                    vbType = "DateTime";
                    break;
                case esriFieldType.esriFieldTypeBlob:
                    csType = "byte[]";
                    vbType = "Byte()";
                    break;
                case esriFieldType.esriFieldTypeGUID:
                    csType = "Guid";
                    vbType = "Guid";
                    break;
                default:
                    return;
            }

            if (isNullable
                && fieldType != esriFieldType.esriFieldTypeString
                && fieldType != esriFieldType.esriFieldTypeBlob)
            {
                csType += "?";
                vbType += "?";
            }

            var propertyName = Regex.Replace(name, @"\W", "_");
            propertyName = _keywords.Contains(propertyName.ToLower()) ? propertyName + "_" : propertyName;

            var indentation = _hasNamespace ? "    " : "";

            _cs.AppendFormat("\r\n{0}    [MappedField(\"{1}\"{2})]\r\n{0}    public virtual {3} {4} {{ get; set; }}\r\n", indentation, name, lengthText, csType, propertyName);
            _vb.AppendFormat("\r\n{0}    <MappedField(\"{1}\"{2})>\r\n{0}    Public Overridable Property {4} As {3}\r\n", indentation, name, lengthText, vbType, propertyName);
        }

        private void AppendEnding()
        {
            if (_hasNamespace)
            {
                _cs.AppendLine("    }\r\n}");
                _vb.AppendLine("\r\n    End Class\r\n\r\nEnd Namespace");
            }
            else
            {
                _cs.AppendLine("}");
                _vb.AppendLine("\r\nEnd Class");
            }
        }

        #endregion

        public CodeGenerator(ITable table, string namespaceName, string className, bool includeUsingStatements)
        {
            _hasNamespace = !string.IsNullOrEmpty(namespaceName);

            if (includeUsingStatements)
                AppendUsingStatements();

            AppendBeginning(namespaceName, className);

            var fields = table.Fields;

            for (var i = 0; i < fields.FieldCount; i++)
            {
                var field = fields.get_Field(i);

                if (field.Editable)
                    AppendProperty(field.Name, field.Type, field.Length, field.IsNullable);
            }

            AppendEnding();
        }

        public string ToCSharp()
        {
            return _cs.ToString();
        }

        public string ToVB()
        {
            return _vb.ToString();
        }
    }
}
