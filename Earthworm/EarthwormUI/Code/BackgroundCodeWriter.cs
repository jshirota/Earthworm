using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using ESRI.ArcGIS.Geodatabase;

namespace EarthwormUI
{
    internal class BackgroundCodeWriter : IDisposable
    {
        private readonly MainWindowViewModel _viewModel;
        private readonly BackgroundWorker _worker;

        public BackgroundCodeWriter(MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            _worker = new BackgroundWorker();
        }

        public void Run()
        {
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += DoWork;
            _worker.ProgressChanged += ProgressChanged;
            _worker.RunWorkerCompleted += RunWorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            _worker.ReportProgress(20, "Connecting...");

            IWorkspace workspace = Utility.GetWorkspace(_viewModel.Geodatabase);

            CodeWriter writer = _viewModel.IsCSharp
                ? new CodeWriterCS(_viewModel.Geodatabase, _viewModel.Folder, _viewModel.ProjectName)
                : new CodeWriterVB(_viewModel.Geodatabase, _viewModel.Folder, _viewModel.ProjectName) as CodeWriter;

            writer.WriteLibraryFiles();
            writer.CopyGeodatabaseFiles();

            _worker.ReportProgress(40, "Generating code...");

            writer.ReadGeodatabase(workspace);
            writer.WriteBusinessObjects();
            writer.WriteProgramFiles();
            writer.WriteAssemblyInfo();
            writer.FinalizeSolution();

            _worker.ReportProgress(80, "Launching Visual Studio...");
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _viewModel.StatusText = e.UserState as string;
            _viewModel.ProgressPercentage = e.ProgressPercentage;
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Process.Start(_viewModel.Folder + "\\" + _viewModel.ProjectName + "\\" + _viewModel.ProjectName + ".sln");
                _viewModel.ProgressPercentage = 100;
                Thread.Sleep(4000);
            }
            catch
            {
                Process.Start(_viewModel.Folder + "\\" + _viewModel.ProjectName);
                MessageBox.Show("You need Visual Studio 2010 or higher to open this.", Utility.Product, MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _viewModel.RaiseProcessEnded();
        }

        public void Dispose()
        {
            _worker.Dispose();
        }
    }
}
