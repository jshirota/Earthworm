using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EarthwormUI.Properties;

namespace EarthwormUI
{
    internal class SolutionGenerator
    {
        private readonly string _guid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
        private readonly string _projectName;
        private readonly string _solutionFolder;
        private readonly string _projectFolder;
        private readonly List<string> _compileFiles = new List<string>();
        private readonly List<string> _copyIfNewerFiles = new List<string>();
        private readonly List<string> _doNotCopyFiles = new List<string>();

        public SolutionGenerator(string folder, string projectName)
        {
            _projectName = projectName;
            _solutionFolder = folder + "\\" + projectName;
            _projectFolder = _solutionFolder + "\\" + projectName;
        }

        public void CreateFolder(string folderName)
        {
            Directory.CreateDirectory(_projectFolder + "\\" + folderName);
        }

        public void WriteCompileFile(string fileName, string content)
        {
            File.WriteAllText(_projectFolder + "\\" + fileName, content);
            _compileFiles.Add(fileName);
        }

        public void AddFile(string folderName, string fileName, bool copyIfNewer)
        {
            string name = folderName + "\\" + Path.GetFileName(fileName);
            File.Copy(fileName, _projectFolder + "\\" + name);

            if (copyIfNewer)
                _copyIfNewerFiles.Add(name);
            else
                _doNotCopyFiles.Add(name);
        }

        public void FinalizeSolution(bool isVB)
        {
            Func<List<string>, string, string> create = (files, format) => string.Join("\r\n", files.Select(f => "    " + string.Format(format, f)).ToArray());

            string version = Utility.Version;

            string compileFiles = create(_compileFiles, "<Compile Include=\"{0}\"/>");
            string copyIfNewerFiles = create(_copyIfNewerFiles, "<None Include=\"{0}\"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>");
            string doNotCopyFiles = create(_doNotCopyFiles, "<None Include=\"{0}\"/>");

            string extension = isVB ? ".vbproj" : ".csproj";
            string guid = isVB ? "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}" : "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

            string project = (isVB ? Resources.vbproj : Resources.csproj).Inject(_guid, _projectName, version, compileFiles, copyIfNewerFiles, doNotCopyFiles);
            File.WriteAllText(_projectFolder + "\\" + _projectName + extension, project);

            string solution = Resources.sln.Inject(_projectName, _guid, extension, guid);
            File.WriteAllText(_projectFolder + ".sln", solution);
        }
    }
}
