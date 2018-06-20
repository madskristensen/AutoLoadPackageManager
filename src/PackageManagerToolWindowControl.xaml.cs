namespace AutoLoadPackageManager
{
    using Microsoft.VisualStudio.Settings;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Settings;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for PackageManagerToolWindowControl.
    /// </summary>
    public partial class PackageManagerToolWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageManagerToolWindowControl"/> class.
        /// </summary>
        public PackageManagerToolWindowControl()
        {
            InitializeComponent();

            // The following code is added for automated tests
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GetSyncPackageList.txt")))
            {
                Button1_Click(button1, null);
            }

        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            SettingsStore configurationStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.Configuration);
            var enumerator = new AutoLoadPackageEnumerator(configurationStore);
            var packageList = enumerator.GetAutoLoadPackages().ToList();

            //tbx_Output.Clear();
            gridOutput.ItemsSource = packageList;
            //var packageListCsv = new StringBuilder();
            //var packageGuidStrings = new List<string>();
            //packageListCsv.AppendLine("AutoLoadContextGuid,ContextName,IsRuleBasedUIContext,PackageGuid,PackageName,ModuleName,IsAsyncPackage,IsAsyncForUIContext,RuleExpression");
            //foreach (AutoLoadPackage package in packageList)
            //{
            //    packageListCsv.AppendLine($"{package.AutoLoadContextGuid},{package.AutoLoadContextName},{package.IsRuleBasedUIContext},{package.PackageGuid},{package.PackageName},{package.ModuleName},{package.IsAsyncPackage},{package.IsAsyncForUIContext},{package.UIContextTerms}");
            //    if (!package.IsAsyncForUIContext)
            //    {
            //        packageGuidStrings.Add(package.PackageGuid.ToString());
            //    }
            //}

            //tbx_Output.Text = packageListCsv.ToString();

            //WriteDataToDesktop(tbx_Output.Text, "PackageList.csv");
            //WriteDataToDesktop(packageGuidStrings.Distinct().OrderBy(x => x).ToArray(), "SyncPackageGuidList.txt");
        }

        private void WriteDataToDesktop(string text, string fileName)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            File.WriteAllText(path, text);
        }

        private void WriteDataToDesktop(string[] lines, string fileName)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            File.WriteAllLines(path, lines);
        }
    }
}