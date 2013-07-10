using System.Linq;
using System.Windows;
using System.Windows.Media;
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

            if (Settings.Default.FirstTime)
                ResetFirstTimeMessage();
            else
                HideFirstTimeMessage();

            Settings.Default.FirstTime = false;
        }

        private void ResetFirstTimeMessage()
        {
            FirstTimeMessage.Text = "Drop a geodatabase here";
            FirstTimeMessage.Foreground = Brushes.LightGray;
            FirstTimeMessage.FontSize = 36;
        }

        private void HideFirstTimeMessage()
        {
            FirstTimeMessage.Visibility = Visibility.Collapsed;
            MainPanel.Visibility = Visibility.Visible;
        }

        private string GetValidFile(DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (files == null || files.Length != 1)
                return null;

            string file = files[0];

            string[] supportedExtensions = { ".gdb", ".mdb", ".sde" };

            if (!supportedExtensions.Any(ext => file.ToLower().EndsWith(ext)))
                return null;

            return file;
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            string file = GetValidFile(e);

            if (file == null)
            {
                FirstTimeMessage.Text = ":(";
                FirstTimeMessage.Foreground = Brushes.OrangeRed;
            }
            else
            {
                FirstTimeMessage.Text = ":)";
                FirstTimeMessage.Foreground = Brushes.ForestGreen;
            }

            FirstTimeMessage.FontSize = 72;
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            ResetFirstTimeMessage();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string file = GetValidFile(e);

            if (file == null)
                return;

            _viewModel.Geodatabase = file;

            txtProjectName.Focus();
            txtProjectName.SelectAll();

            HideFirstTimeMessage();
        }
    }
}
