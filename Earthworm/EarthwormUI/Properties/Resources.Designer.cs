﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18047
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EarthwormUI.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EarthwormUI.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.IO;
        ///using System.Linq;
        ///using System.Reflection;
        ///using ESRI.ArcGIS.esriSystem;
        ///
        ///namespace `0`
        ///{
        ///    class AssemblyManager
        ///    {
        ///        private static readonly string[] DllPaths;
        ///        private static AssemblyManager _instance;
        ///        private AoInitialize _ao;
        ///
        ///        /// &lt;summary&gt;
        ///        /// Binds to Desktop or Engine and initializes the first available license.  This also attempts to resolve Esri assembly paths so that a program compiled using 10.1 can run on 9 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AssemblyManager_cs {
            get {
                return ResourceManager.GetString("AssemblyManager_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Imports System.IO
        ///Imports ESRI.ArcGIS.esriSystem
        ///
        ///Public Class AssemblyManager
        ///
        ///    Private Shared ReadOnly DllPaths As String()
        ///    Private Shared _instance As AssemblyManager
        ///    Private _ao As AoInitialize
        ///
        ///    &apos;&apos;&apos; &lt;summary&gt;
        ///    &apos;&apos;&apos; Binds to Desktop or Engine and initializes the first available license.  This also attempts to resolve Esri assembly paths so that a program compiled using 10.1 can run on 9.3.1 machines.  Call this from the static constructor of the main class.
        ///    &apos;&apos;&apos; &lt;/summary&gt;        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AssemblyManager_vb {
            get {
                return ResourceManager.GetString("AssemblyManager_vb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;Project ToolsVersion=&quot;4.0&quot; DefaultTargets=&quot;Build&quot; xmlns=&quot;http://schemas.microsoft.com/developer/msbuild/2003&quot;&gt;
        ///  &lt;PropertyGroup&gt;
        ///    &lt;Configuration Condition=&quot; &apos;$(Configuration)&apos; == &apos;&apos; &quot;&gt;Debug&lt;/Configuration&gt;
        ///    &lt;Platform Condition=&quot; &apos;$(Platform)&apos; == &apos;&apos; &quot;&gt;x86&lt;/Platform&gt;
        ///    &lt;ProjectGuid&gt;`0`&lt;/ProjectGuid&gt;
        ///    &lt;OutputType&gt;Exe&lt;/OutputType&gt;
        ///    &lt;AppDesignerFolder&gt;Properties&lt;/AppDesignerFolder&gt;
        ///    &lt;RootNamespace&gt;`1`&lt;/RootNamespace&gt;
        ///    &lt;AssemblyName&gt;`1`&lt;/Assembl [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string csproj {
            get {
                return ResourceManager.GetString("csproj", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Imports Earthworm
        ///Imports Earthworm.AO
        ///Imports Earthworm.Serialization
        ///
        ///Module Module1
        ///
        ///    Sub New()
        ///
        ///        AssemblyManager.InitializeLicense()
        ///
        ///    End Sub
        ///
        ///    Sub Main()
        ///`0`
        ///        Dim featureWorkspace = DirectCast(workspace, IFeatureWorkspace)
        ///`1`
        ///        AssemblyManager.ReleaseLicense()
        ///
        ///        Console.WriteLine(&quot;Press ENTER to exit&quot;)
        ///        Console.ReadLine()
        ///
        ///    End Sub
        ///
        ///End Module
        ///.
        /// </summary>
        internal static string Module1_vb {
            get {
                return ResourceManager.GetString("Module1_vb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Collections.Generic;
        ///using System.Linq;
        ///using ESRI.ArcGIS.DataSourcesGDB;
        ///using ESRI.ArcGIS.esriSystem;
        ///using ESRI.ArcGIS.Geodatabase;
        ///using ESRI.ArcGIS.Geometry;
        ///using Earthworm;
        ///using Earthworm.AO;
        ///using Earthworm.Serialization;
        ///
        ///namespace `0`
        ///{
        ///    class Program
        ///    {
        ///        static Program()
        ///        {
        ///            AssemblyManager.InitializeLicense();
        ///        }
        ///
        ///        [STAThread]
        ///        static void Main()
        ///        {`1`
        ///            IFeatureWorkspace featu [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Program_cs {
            get {
                return ResourceManager.GetString("Program_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ﻿
        ///Microsoft Visual Studio Solution File, Format Version 11.00
        ///# Visual Studio 2010
        ///Project(&quot;`3`&quot;) = &quot;`0`&quot;, &quot;`0`\`0``2`&quot;, &quot;`1`&quot;
        ///EndProject
        ///Global
        ///	GlobalSection(SolutionConfigurationPlatforms) = preSolution
        ///		Debug|x86 = Debug|x86
        ///		Release|x86 = Release|x86
        ///	EndGlobalSection
        ///	GlobalSection(ProjectConfigurationPlatforms) = postSolution
        ///		`1`.Debug|x86.ActiveCfg = Debug|x86
        ///		`1`.Debug|x86.Build.0 = Debug|x86
        ///		`1`.Release|x86.ActiveCfg = Release|x86
        ///		`1`.Release|x86.Build.0 = Release|x86
        ///	End [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string sln {
            get {
                return ResourceManager.GetString("sln", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;Project ToolsVersion=&quot;4.0&quot; DefaultTargets=&quot;Build&quot; xmlns=&quot;http://schemas.microsoft.com/developer/msbuild/2003&quot;&gt;
        ///  &lt;PropertyGroup&gt;
        ///    &lt;Configuration Condition=&quot; &apos;$(Configuration)&apos; == &apos;&apos; &quot;&gt;Debug&lt;/Configuration&gt;
        ///    &lt;Platform Condition=&quot; &apos;$(Platform)&apos; == &apos;&apos; &quot;&gt;x86&lt;/Platform&gt;
        ///    &lt;ProjectGuid&gt;`0`&lt;/ProjectGuid&gt;
        ///    &lt;OutputType&gt;Exe&lt;/OutputType&gt;
        ///    &lt;StartupObject&gt;`1`.Module1&lt;/StartupObject&gt;
        ///    &lt;RootNamespace&gt;`1`&lt;/RootNamespace&gt;
        ///    &lt;AssemblyName&gt;`1`&lt;/AssemblyName&gt;        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string vbproj {
            get {
                return ResourceManager.GetString("vbproj", resourceCulture);
            }
        }
    }
}
