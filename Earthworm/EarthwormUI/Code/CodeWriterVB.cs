using System;
using System.IO;
using System.Text;
using Earthworm.Meta;
using EarthwormUI.Properties;

namespace EarthwormUI
{
    internal class CodeWriterVB : CodeWriter
    {
        public CodeWriterVB(string geodatabase, string folder, string projectName)
        {
            Geodatabase = geodatabase;
            ProjectName = projectName;
            Generator = new SolutionGenerator(folder, projectName);
        }

        public override void WriteBusinessObjects()
        {
            Generator.CreateFolder("BusinessObjects");

            foreach (GeodatabaseItem item in Items)
                Generator.WriteCompileFile(@"BusinessObjects\" + item.ClassName + ".vb", item.Table.ToVB(item.ClassName));
        }

        public override void WriteProgramFiles()
        {
            string fileName = Path.GetFileName(Geodatabase);

            string factory;

            if (fileName.ToLower().EndsWith(".gdb"))
                factory = "New FileGDBWorkspaceFactory";
            else if (fileName.ToLower().EndsWith(".mdb"))
                factory = "New AccessWorkspaceFactory";
            else if (fileName.ToLower().EndsWith(".sde"))
                factory = "New SdeWorkspaceFactory";
            else
                throw new Exception("Invalid file format.");

            string code = @"
        Dim workspaceFactory As {0}
        Dim workspace = workspaceFactory.OpenFromFile(""Data\{1}"", 0)";

            string workspaceCode = string.Format(code, factory, fileName);

            WriteCompleteProgramFiles(workspaceCode);
        }

        private void WriteCompleteProgramFiles(string workspaceCode)
        {
            StringBuilder code = new StringBuilder();

            foreach (GeodatabaseItem item in Items)
            {
                code.AppendFormat(@"
        Dim {1} = featureWorkspace.Open{0}(""{2}"")

        For Each {4} in {1}.Map(OF {3}).Take(5)
            Console.WriteLine({4})
        Next
", item.IsSpatial ? "FeatureClass" : "Table", item.TableVariableName, item.DatasetName, item.ClassName, item.VariableName);
            }

            string program = Resources.Module1_vb.Inject(workspaceCode, code);
            Generator.WriteCompileFile("Module1.vb", program);
            Generator.WriteCompileFile("AssemblyManager.vb", Resources.AssemblyManager_vb);
        }

        public override void WriteAssemblyInfo()
        {
            Generator.CreateFolder("My Project");

            string code = @"Imports System.Reflection
Imports System.Runtime.InteropServices
<Assembly: AssemblyVersionAttribute(""1.0.0.0"")>
<Assembly: ComVisibleAttribute(False)>";

            Generator.WriteCompileFile("My Project\\AssemblyInfo.vb", code);
        }

        public override void FinalizeSolution()
        {
            Generator.FinalizeSolution(true);
        }
    }
}
