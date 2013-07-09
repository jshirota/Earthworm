using System.Linq;
using System.Windows;
using EarthwormUI.Properties;

namespace EarthwormUI
{
    public partial class MainWindow
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            _viewModel.ProcessEnded += (s, e) => Application.Current.Shutdown();

            DataContext = _viewModel;

            if (!Settings.Default.FirstTime)
                ShowControls();

            Settings.Default.FirstTime = false;
        }

        private void ShowControls()
        {
            DragBoxPanel.Visibility = Visibility.Collapsed;
            MainPanel.Visibility = Visibility.Visible;
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (files == null || files.Length != 1)
                return;

            string file = files[0];

            string[] supportedExtensions = { ".gdb", ".mdb", ".sde" };

            if (!supportedExtensions.Any(ext => file.ToLower().EndsWith(ext)))
                return;

            _viewModel.Geodatabase = file;

            txtProjectName.Focus();
            txtProjectName.SelectAll();

            ShowControls();
        }
    }
}
