using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace ForceStep
{
    internal sealed class ManuallyDisable
    {
        public const int CommandId = 4132;

        public static readonly Guid CommandSet = new Guid("5499ab04-087c-4366-bd73-ff583b883993");

        private readonly AsyncPackage package;

        private ManuallyDisable(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);

            menuItem.BeforeQueryStatus += OnBeforeQueryStatus;

        }

        public static ManuallyDisable Instance
        {
            get;
            private set;
        }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in ManuallyDisable's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ManuallyDisable(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var bpm = new BreakpointManager(package);
            bpm.SaveAndSuspendActiveBreakpoints(ForceStepConstants.SaveBreakpointReason.Manual);
        }

        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte = UtilityMethods.GetDTE(package);
            if (sender is OleMenuCommand menuCommand)
            {
                menuCommand.Enabled = (dte.Debugger.Breakpoints.Count > 0);
            }
        }

    }
}
