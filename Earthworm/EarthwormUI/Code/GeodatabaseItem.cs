using System.Text.RegularExpressions;
using ESRI.ArcGIS.Geodatabase;

namespace EarthwormUI
{
    internal class GeodatabaseItem
    {
        public ITable Table { get; private set; }
        public bool IsSpatial { get; private set; }
        public string DatasetName { get; private set; }
        public string ClassName { get; private set; }
        public string VariableName { get; private set; }
        public string TableVariableName { get; private set; }

        public GeodatabaseItem(IDataset dataset, string className)
        {
            Table = dataset as ITable;
            IsSpatial = dataset.Type == esriDatasetType.esriDTFeatureClass;
            DatasetName = dataset.Name;
            ClassName = className;
            VariableName = ClassName.ToSingular(false);
            TableVariableName = Regex.Replace(VariableName, "_*?$", "") + (IsSpatial ? "FeatureClass" : "Table");
        }
    }
}
