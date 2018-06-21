namespace AutoLoadPackageManager
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Settings;

    [Guid(WindowGuid)]
    public class PackageManagerToolWindow : ToolWindowPane
    {
        public const string WindowGuid = "ddac3bc3-cd4d-4eb1-b726-af3a4402667f";
        public const string Title = "Package Load Explorer";

        public PackageManagerToolWindow(ShellSettingsManager settingsManager) : base(null)
        {
            Caption = Title;
            Content = new PackageManagerToolWindowControl(settingsManager);
        }
    }
}
