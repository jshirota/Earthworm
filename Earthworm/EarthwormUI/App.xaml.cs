using System.Windows;

namespace EarthwormUI
{
    public partial class App
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            AssemblyManager.ReleaseLicense();
        }
    }
}
