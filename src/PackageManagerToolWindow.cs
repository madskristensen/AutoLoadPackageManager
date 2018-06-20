namespace AutoLoadPackageManager
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    [Guid(WindowGuid)]
    public class PackageManagerToolWindow : ToolWindowPane
    {
        public const string WindowGuid = "ddac3bc3-cd4d-4eb1-b726-af3a4402667f";
        public const string Title = "VS Package Load Explorer";
        public PackageManagerToolWindow() : base(null)
        {
            Caption = Title;

            Content = new PackageManagerToolWindowControl();
        }
    }
}
