namespace AutoLoadPackageManager
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.VisualStudio.Settings;
    using Microsoft.VisualStudio.Shell.Settings;

    public partial class PackageManagerToolWindowControl : UserControl
    {
        private readonly ShellSettingsManager _settingsManager;

        public PackageManagerToolWindowControl(ShellSettingsManager settingsManager)
        {
            InitializeComponent();
            _settingsManager = settingsManager;

            Loaded += (s, e) => {
                if (gridOutput.ItemsSource == null)
                {
                    Button1_Click(this, null);
                }
            };
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            SettingsStore configurationStore = _settingsManager.GetReadOnlySettingsStore(SettingsScope.Configuration);
            var enumerator = new AutoLoadPackageEnumerator(configurationStore);
            var packageList = enumerator.GetAutoLoadPackages().ToList();

            gridOutput.ItemsSource = packageList;
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