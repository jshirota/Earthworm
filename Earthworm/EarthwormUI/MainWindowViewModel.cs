using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using EarthwormUI.Properties;

namespace EarthwormUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _licenseInitialized;
        private string _geodatabase;
        private string _folder;
        private string _projectName;
        private bool _isProcessing;
        private string _statusText;
        private int _progressPercentage;

        public MainWindowViewModel()
        {
            StatusText = "Initializing license...";

            Task.Factory.StartNew(() =>
            {
                AssemblyManager.InitializeLicense();
                _licenseInitialized = true;
                StatusText = "Ready";

                if (Application.Current != null)
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)CommandManager.InvalidateRequerySuggested);
            });

            Folder = Directory.Exists(Settings.Default.Folder) ? Settings.Default.Folder : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            IsCSharp = Settings.Default.IsCSharp;
        }

        public event EventHandler ProcessEnded;
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaiseProcessEnded()
        {
            if (ProcessEnded != null)
                ProcessEnded(this, EventArgs.Empty);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Properties

        public string Geodatabase
        {
            get { return _geodatabase; }
            set
            {
                _geodatabase = value;
                RaisePropertyChanged("Geodatabase");

                if (string.IsNullOrEmpty(_geodatabase))
                    return;

                string name = Path.GetFileNameWithoutExtension(_geodatabase);
                name = Regex.Replace(name, @"\W", "") + "Project";

                ProjectName = name.ToSafeName(n => !Directory.Exists(Folder + "\\" + n));
            }
        }

        public string Folder
        {
            get { return _folder; }
            set
            {
                _folder = value;
                RaisePropertyChanged("Folder");
            }
        }

        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                _projectName = value;
                RaisePropertyChanged("ProjectName");
            }
        }

        public bool IsCSharp { get; set; }

        public bool IsNotCSharp
        {
            get { return !IsCSharp; }
        }

        public bool IsProcessing
        {
            get { return _isProcessing; }
            set
            {
                _isProcessing = value;
                RaisePropertyChanged("IsProcessing");
                RaisePropertyChanged("IsNotProcessing");
            }
        }

        public bool IsNotProcessing
        {
            get { return !IsProcessing; }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                RaisePropertyChanged("StatusText");
                RaisePropertyChanged("TitleText");
            }
        }

        public string TitleText
        {
            get { return StatusText == "Ready" ? "Earthworm" : StatusText; }
        }

        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            set
            {
                _progressPercentage = value;
                RaisePropertyChanged("ProgressPercentage");
            }
        }

        #endregion

        #region Commands

        public ICommand CreateCommand
        {
            get { return new RelayCommand(Create, CanCreate); }
        }

        private void Create()
        {
            IsProcessing = true;

            Settings.Default.IsCSharp = IsCSharp;
            Settings.Default.Folder = Folder;
            Settings.Default.Save();

            using (BackgroundCodeWriter writer = new BackgroundCodeWriter(this))
                writer.Run();
        }

        private bool CanCreate()
        {
            return _licenseInitialized
                && (Directory.Exists(Geodatabase) || File.Exists(Geodatabase))
                && Directory.Exists(Folder)
                && !Directory.Exists(Folder + "\\" + ProjectName);
        }

        public ICommand OpenSampleDataFolderCommand
        {
            get
            {
                string f = Utility.GetSampleDataFolder();
                return new RelayCommand(() => Process.Start(f), () => Directory.Exists(f));
            }
        }

        public ICommand OpenArcCatalogFolderCommand
        {
            get
            {
                string f = Utility.GetArcCatalogFolder();
                return new RelayCommand(() => Process.Start(f), () => Directory.Exists(f));
            }
        }

        #endregion
    }
}
