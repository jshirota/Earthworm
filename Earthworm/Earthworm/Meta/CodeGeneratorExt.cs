using ESRI.ArcGIS.Geodatabase;

namespace Earthworm.Meta
{
    /// <summary>
    /// Provides extension methods that return CSharp or VB code representations of geodatabase objects.
    /// </summary>
    public static class CodeGeneratorExt
    {
        /// <summary>
        /// Generates a CSharp class that maps this table into an object.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="namespaceName"></param>
        /// <param name="className"></param>
        /// <param name="includeUsingStatements"></param>
        /// <returns></returns>
        public static string ToCSharp(this ITable table, string namespaceName, string className, bool includeUsingStatements = true)
        {
            return new CodeGenerator(table, namespaceName, className, includeUsingStatements).ToCSharp();
        }

        /// <summary>
        /// Generates a CSharp class that maps this table into an object.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="className"></param>
        /// <param name="includeUsingStatements"></param>
        /// <returns></returns>
        public static string ToCSharp(this ITable table, string className, bool includeUsingStatements = true)
        {
            return table.ToCSharp(null, className, includeUsingStatements);
        }

        /// <summary>
        /// Generates a CSharp class that maps this feature class into an object.
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="namespaceName"></param>
        /// <param name="className"></param>
        /// <param name="includeUsingStatements"></param>
        /// <returns></returns>
        public static string ToCSharp(this IFeatureClass featureClass, string namespaceName, string className, bool includeUsingStatements = true)
        {
            return ((ITable)featureClass).ToCSharp(namespaceName, className, includeUsingStatements);
        }

        /// <summary>
        /// Generates a CSharp class that maps this feature class into an object.
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="className"></param>
        /// <param name="includeUsingStatements"></param>
        /// <returns></returns>
        public static string ToCSharp(this IFeatureClass featureClass, string className, bool includeUsingStatements = true)
        {
            return featureClass.ToCSharp(null, className, includeUsingStatements);
        }

        /// <summary>
        /// Generates a VB class that maps this table into an object.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="namespaceName"></param>
        /// <param name="className"></param>
        /// <param name="includeUsingStatements"></param>
        /// <returns></returns>
        public static string ToVB(this ITable table, string namespaceName, string className, bool includeUsingStatements = true)
        {
            return new CodeGenerator(table, namespaceName, className, includeUsingStatements).ToVB();
        }

        /// <summary>
        /// Generates a VB class that maps this table into an object.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="className"></param>
        /// <param name="includeUsingStatements"></param>
        /// <returns></returns>
        public static string ToVB(this ITable table, string className, bool includeUsingStatements = true)
        {
            return table.ToVB(null, className, includeUsingStatements);
        }

        /// <summary>
        /// Generates a VB class that maps this feature class into an object.
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="namespaceName"></param>
        /// <param name="className"></param>
        /// <param name="includeUsingStatements"></param>
        /// <returns></returns>
        public static string ToVB(this IFeatureClass featureClass, string namespaceName, string className, bool includeUsingStatements = true)
        {
            return ((ITable)featureClass).ToVB(namespaceName, className, includeUsingStatements);
        }

        /// <summary>
        /// Generates a VB class that maps this feature class into an object.
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="className"></param>
        /// <param name="includeUsingStatements"></param>
        /// <returns></returns>
        public static string ToVB(this IFeatureClass featureClass, string className, bool includeUsingStatements = true)
        {
            return featureClass.ToVB(null, className, includeUsingStatements);
        }
    }
}
