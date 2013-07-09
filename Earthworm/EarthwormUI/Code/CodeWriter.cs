using System.Collections.Generic;
using System.IO;
using System.Linq;
using ESRI.ArcGIS.Geodatabase;

namespace EarthwormUI
{
    internal abstract class CodeWriter
    {
        protected string Geodatabase;
        protected string ProjectName;
        protected SolutionGenerator Generator;
        protected List<GeodatabaseItem> Items;

        public void WriteLibraryFiles()
        {
            Generator.CreateFolder("Library");

            Generator.AddFile(@"Library", "Earthworm.dll", false);

            string xmlFile = "Earthworm.xml";

            if (File.Exists(xmlFile))
                Generator.AddFile(@"Library", xmlFile, false);
        }

        public void CopyGeodatabaseFiles()
        {
            Generator.CreateFolder("Data");

            if (Geodatabase.ToLower().EndsWith(".gdb"))
            {
                string folderName = "Data\\" + Path.GetFileName(Geodatabase);
                Generator.CreateFolder(folderName);

                foreach (string file in Directory.GetFiles(Geodatabase, "*"))
                    if (!file.EndsWith(".lock"))
                        Generator.AddFile(folderName, file, true);
            }
            else
            {
                Generator.AddFile("Data", Geodatabase, true);
            }
        }

        public void ReadGeodatabase(IWorkspace workspace)
        {
            List<string> classNames = new List<string>();

            IEnumerable<GeodatabaseItem> items = workspace.GetDatasets(esriDatasetType.esriDTAny, true)
                .Where(ds => (ds.Type == esriDatasetType.esriDTFeatureClass || ds.Type == esriDatasetType.esriDTTable) && ((ITable)ds).HasOID)
                .Select(ds =>
                {
                    string className = ds.Name.Split('.').Last().ToSingular(true);
                    className = className.ToSafeName(n => !classNames.Contains(n));
                    classNames.Add(className);

                    return new GeodatabaseItem(ds, className);
                });

            Items = items.ToList();
        }

        public abstract void WriteBusinessObjects();
        public abstract void WriteProgramFiles();
        public abstract void WriteAssemblyInfo();
        public abstract void FinalizeSolution();
    }
}
