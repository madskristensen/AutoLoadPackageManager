using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace AutoLoadPackageManager
{
    internal sealed class PackageManagerToolWindowCommand
    {
        private readonly AsyncPackage _package;

        private PackageManagerToolWindowCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            var menuCommandID = new CommandID(PackageGuids.guidAutoLoadPackageManagerCmdSet, PackageIds.PackageManagerToolWindowCommandId);
            var menuItem = new MenuCommand(ShowToolWindow, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static PackageManagerToolWindowCommand Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get
            {
                return _package;
            }
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new PackageManagerToolWindowCommand(package, commandService);
        }

        private void ShowToolWindow(object sender, EventArgs e)
        {
            _package.JoinableTaskFactory.RunAsync(async () =>
            {
                ToolWindowPane window = await _package.ShowToolWindowAsync(
                    toolWindowType: typeof(PackageManagerToolWindow),
                    id: 0,
                    create: true,
                    cancellationToken: _package.DisposalToken);
            });
        }
    }
}
