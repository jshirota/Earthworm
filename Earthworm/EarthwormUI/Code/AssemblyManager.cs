using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ESRI.ArcGIS.esriSystem;

namespace EarthwormUI
{
    internal class AssemblyManager
    {
        private static readonly string[] DllPaths;
        private static AssemblyManager _instance;
        private AoInitialize _ao;

        public static void InitializeLicense()
        {
            if (_instance == null)
                _instance = new AssemblyManager();
        }

        public static void ReleaseLicense()
        {
            if (_instance != null)
            {
                _instance.Shutdown();
                _instance = null;
            }
        }

        private void RuntimeBind()
        {
            string dll = DllPaths.FirstOrDefault(p => p.Contains(@"\ESRI.ArcGIS.Version\"));

            if (dll != null)
                Assembly.LoadFrom(dll)
                    .GetExportedTypes()
                    .Single(t => t.Name == "RuntimeManager")
                    .GetMethod("Bind")
                    .Invoke(null, new object[] { 100 });
        }

        private void Initialize()
        {
            _ao = new AoInitialize();

            int[] licenseCodes = { 60, 50, 40, 10 };

            int licenseCode = licenseCodes
                .FirstOrDefault(c => _ao.IsProductCodeAvailable((esriLicenseProductCode)c) == esriLicenseStatus.esriLicenseAvailable);

            if (licenseCode == 0)
                throw new Exception("No license available.");

            _ao.Initialize((esriLicenseProductCode)licenseCode);
        }

        private void Shutdown()
        {
            try
            {
                lock (_ao)
                {
                    if (_ao != null)
                    {
                        _ao.Shutdown();
                        _ao = null;
                    }
                }
            }
            catch { }
        }

        static AssemblyManager()
        {
            DllPaths = Directory.GetFiles(Environment.GetEnvironmentVariable("windir") + @"\assembly", "*ESRI*.dll", SearchOption.AllDirectories);

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string dll = DllPaths.FirstOrDefault(p => p.Contains(@"\" + args.Name.Split(',')[0] + @"\"));

                if (dll == null)
                    return null;

                return Assembly.LoadFrom(dll);
            };
        }

        private AssemblyManager()
        {
            RuntimeBind();
            Initialize();
        }

        ~AssemblyManager()
        {
            Shutdown();
        }
    }
}
