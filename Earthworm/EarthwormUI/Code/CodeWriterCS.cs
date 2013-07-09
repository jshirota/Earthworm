using System;
using System.IO;
using System.Text;
using Earthworm.Meta;
using EarthwormUI.Properties;

namespace EarthwormUI
{
    internal class CodeWriterCS : CodeWriter
    {
        public CodeWriterCS(string geodatabase, string folder, string projectName)
        {
            Geodatabase = geodatabase;
            ProjectName = projectName;
            Generator = new SolutionGenerator(folder, projectName);
        }

        public override void WriteBusinessObjects()
        {
            Generator.CreateFolder("BusinessObjects");

            foreach (GeodatabaseItem item in Items)
                Generator.WriteCompileFile(@"BusinessObjects\" + item.ClassName + ".cs", item.Table.ToCSharp(ProjectName, item.ClassName));
        }

        public override void WriteProgramFiles()
        {
            string fileName = Path.GetFileName(Geodatabase);

            string factory;

            if (fileName.ToLower().EndsWith(".gdb"))
                factory = "new FileGDBWorkspaceFactory()";
            else if (fileName.ToLower().EndsWith(".mdb"))
                factory = "new AccessWorkspaceFactory()";
            else if (fileName.ToLower().EndsWith(".sde"))
                factory = "new SdeWorkspaceFactory()";
            else
                throw new Exception("Invalid file format.");

            string code = @"
            IWorkspaceFactory workspaceFactory = {0};
            IWorkspace workspace = workspaceFactory.OpenFromFile(@""Data\{1}"", 0);";

            string workspaceCode = string.Format(code, factory, fileName);

            WriteCompleteProgramFiles(workspaceCode);
        }

        private void WriteCompleteProgramFiles(string workspaceCode)
        {
            StringBuilder code = new StringBuilder();

            foreach (GeodatabaseItem item in Items)
            {
                code.AppendFormat(@"
            I{0} {1} = featureWorkspace.Open{0}(""{2}"");

            foreach ({3} {4} in {1}.Map<{3}>().Take(5))
            {{
                Console.WriteLine({4});
            }}
", item.IsSpatial ? "FeatureClass" : "Table", item.TableVariableName, item.DatasetName, item.ClassName, item.VariableName);
            }

            string program = Resources.Program_cs.Inject(ProjectName, workspaceCode, code);
            Generator.WriteCompileFile("Program.cs", program);
            Generator.WriteCompileFile("AssemblyManager.cs", Resources.AssemblyManager_cs.Inject(ProjectName));
        }

        public override void WriteAssemblyInfo()
        {
            Generator.CreateFolder("Properties");

            string code = @"using System.Reflection;
using System.Runtime.InteropServices;
[assembly: AssemblyVersionAttribute(""1.0.0.0"")]
[assembly: ComVisibleAttribute(false)]";

            Generator.WriteCompileFile("Properties\\AssemblyInfo.cs", code);
        }

        public override void FinalizeSolution()
        {
            Generator.FinalizeSolution(false);
        }
    }
}
