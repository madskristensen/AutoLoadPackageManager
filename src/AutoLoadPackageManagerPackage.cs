using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Task = System.Threading.Tasks.Task;

namespace AutoLoadPackageManager
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [Guid(PackageGuids.guidAutoLoadPackageManagerString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(PackageManagerToolWindow), Style = VsDockStyle.Tabbed, Window = "DocumentWell", Orientation = ToolWindowOrientation.none)]
    public sealed class AutoLoadPackageManagerPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            await PackageManagerToolWindowCommand.InitializeAsync(this);
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            return toolWindowType.Equals(new Guid(PackageManagerToolWindow.WindowGuid))
                ? (this)
                : null;
        }

        protected override string GetToolWindowTitle(Type toolWindowType, int id)
        {
            return toolWindowType == typeof(PackageManagerToolWindow)
                ? PackageManagerToolWindow.Title
                : base.GetToolWindowTitle(toolWindowType, id);
        }

        protected override async Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            var mgr = await GetServiceAsync(typeof(SVsSettingsManager)) as IVsSettingsManager;
            return new ShellSettingsManager(mgr);
        }
    }
}
