using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;

namespace EarthwormUI
{
    internal static class Utility
    {
        #region Assembly Info

        private static string GetAttributeValue<T>(Func<T, string> getValue) where T : Attribute
        {
            T attribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
            return attribute == null ? "" : getValue(attribute);
        }

        public static string Product
        {
            get
            {
                return GetAttributeValue<AssemblyProductAttribute>(a => a.Product);
            }
        }

        public static string Version
        {
            get
            {
                return GetAttributeValue<AssemblyFileVersionAttribute>(a => a.Version);
            }
        }

        #endregion

        #region ArcGIS

        public static IWorkspace GetWorkspace(string fileName)
        {
            IWorkspaceFactory workspaceFactory;

            if (fileName.ToLower().EndsWith(".gdb"))
                workspaceFactory = new FileGDBWorkspaceFactory();
            else if (fileName.ToLower().EndsWith(".mdb"))
                workspaceFactory = new AccessWorkspaceFactory();
            else if (fileName.ToLower().EndsWith(".sde"))
                workspaceFactory = new SdeWorkspaceFactory();
            else
                throw new Exception("Invalid file format.");

            return workspaceFactory.OpenFromFile(fileName, 0);
        }

        private static IEnumerable<IDataset> Enumerate(this IEnumDataset enumDataset)
        {
            IDataset dataset;

            while (true)
            {
                dataset = enumDataset.Next();

                if (dataset == null)
                    yield break;

                yield return dataset;
            }
        }

        public static IEnumerable<IDataset> GetDatasets(this IWorkspace workspace, esriDatasetType datasetType, bool recursive)
        {
            IEnumerable<IDataset> datasets = workspace.get_Datasets(datasetType).Enumerate();

            if (recursive && datasetType != esriDatasetType.esriDTFeatureDataset && datasetType != esriDatasetType.esriDTTable)
            {
                IEnumerable<IDataset> featureDatasets = workspace.get_Datasets(esriDatasetType.esriDTFeatureDataset).Enumerate();
                datasets = datasets.Concat(featureDatasets.SelectMany(d => d.GetDatasets(datasetType)));
            }

            return datasets;
        }

        public static IEnumerable<IDataset> GetDatasets(this IDataset dataset, esriDatasetType datasetType)
        {
            return dataset.Subsets.Enumerate().Where(d => datasetType == esriDatasetType.esriDTAny || d.Type == datasetType);
        }

        public static string GetArcGISVersion()
        {
            string systemDll = Directory.GetFiles(Environment.GetEnvironmentVariable("windir") + @"\assembly", "*ESRI*.dll", SearchOption.AllDirectories)
                .First(p => p.Contains(@"\ESRI.ArcGIS.System\"));

            if (systemDll.Contains("9.3."))
                return "9.3.1";

            string[] versions = { "10.2", "10.1", "10.0" };

            return versions.First(v => systemDll.Contains(v + "."));
        }

        public static string GetSampleDataFolder()
        {
            string arcGISVersion = GetArcGISVersion();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            if (path == "")
                path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            if (arcGISVersion == "10.2")
                return path + @"\ArcGIS\DeveloperKit10.2\Samples\data";

            if (arcGISVersion == "10.1")
                return path + @"\ArcGIS\DeveloperKit10.1\Samples\data";

            if (arcGISVersion == "10.0")
                return path + @"\ArcGIS\DeveloperKit10.0\Samples\data";

            return path + @"\ArcGIS\DeveloperKit\SamplesNET\data";
        }

        public static string GetArcCatalogFolder()
        {
            string arcGISVersion = GetArcGISVersion();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (arcGISVersion == "10.2")
                return path + @"\ESRI\Desktop10.2\ArcCatalog";

            if (arcGISVersion == "10.1")
                return path + @"\ESRI\Desktop10.1\ArcCatalog";

            if (arcGISVersion == "10.0")
                return path + @"\ESRI\Desktop10.0\ArcCatalog";

            return path + @"\ESRI\ArcCatalog";
        }

        #endregion

        #region Text

        private static readonly string[] Keywords = { "abstract", "add", "addhandler", "addressof", "aggregate", "alias", "and", "andalso", "ansi", "as", "ascending", "assembly", "async", "auto", "await", "base", "binary", "bool", "boolean", "break", "by", "byref", "byte", "byval", "call", "case", "catch", "cbool", "cbyte", "cchar", "cdate", "cdbl", "cdec", "char", "checked", "cint", "class", "clng", "cobj", "compare", "const", "continue", "csbyte", "cshort", "csng", "cstr", "ctype", "cuint", "culng", "cushort", "custom", "date", "decimal", "declare", "default", "delegate", "descending", "dim", "directcast", "distinct", "do", "double", "dynamic", "each", "else", "elseif", "end", "endif", "enum", "equals", "erase", "error", "event", "exit", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "friend", "from", "function", "get", "gettype", "getxmlnamespace", "global", "gosub", "goto", "group", "handles", "if", "implements", "implicit", "imports", "in", "inherits", "int", "integer", "interface", "internal", "into", "is", "isfalse", "isnot", "istrue", "iterator", "join", "key", "let", "lib", "like", "lock", "long", "loop", "me", "mid", "mod", "module", "mustinherit", "mustoverride", "mybase", "myclass", "namespace", "narrowing", "new", "next", "not", "nothing", "notinheritable", "notoverridable", "null", "object", "of", "off", "on", "operator", "option", "optional", "or", "order", "orderby", "orelse", "out", "overloads", "overridable", "override", "overrides", "paramarray", "params", "partial", "preserve", "private", "property", "protected", "public", "raiseevent", "readonly", "redim", "ref", "rem", "remove", "removehandler", "resume", "return", "sbyte", "sealed", "select", "set", "shadows", "shared", "short", "single", "sizeof", "skip", "stackalloc", "static", "step", "stop", "strict", "string", "struct", "structure", "sub", "switch", "synclock", "take", "text", "then", "this", "throw", "to", "true", "try", "trycast", "typeof", "uint", "uinteger", "ulong", "unchecked", "unicode", "unsafe", "until", "ushort", "using", "value", "var", "variant", "virtual", "void", "volatile", "wend", "when", "where", "while", "widening", "with", "withevents", "writeonly", "xor", "yield" };

        public static string ToSingular(this string name, bool capital)
        {
            string text = Regex.Replace(name, @"_+[^_]", m => m.Value.Substring(m.Length - 1).ToUpper());

            if (capital)
                text = text.Substring(0, 1).ToUpper() + text.Substring(1);
            else
                text = text.Substring(0, 1).ToLower() + text.Substring(1);

            if (text.EndsWith("ies"))
                text = Regex.Replace(text, @"ies$", "y");

            if (text.EndsWith("s"))
                text = Regex.Replace(text, @"s$", "");

            return Keywords.Contains(text.ToLower()) ? text + "_" : text;
        }

        public static string ToSafeName(this string name, Func<string, bool> condition)
        {
            return Enumerable.Range(0, 100)
                .Select(n => name + (n == 0 ? "" : n.ToString()))
                .First(condition);
        }

        public static string Inject(this string text, params object[] parameters)
        {
            return Regex.Replace(text, @"`\d+`", m =>
            {
                int n = int.Parse(Regex.Replace(m.Value, @"\D", ""));
                return parameters[n].ToString();
            });
        }

        #endregion
    }
}
