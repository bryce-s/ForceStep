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
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ForceStepCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("5499ab04-087c-4366-bd73-ff583b883993");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForceStepCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ForceStepCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);

            menuItem.BeforeQueryStatus += OnBeforeQueryStatus;

            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ForceStepCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in ForceStepCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ForceStepCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = UtilityMethods.GetDTE(package);

            if (dte.Mode == EnvDTE.vsIDEMode.vsIDEModeDebug && dte.Debugger.CurrentMode == EnvDTE.dbgDebugMode.dbgBreakMode)
            {
                var bpm = new BreakpointManager(package);
                bpm.SaveAndSuspendActiveBreakpoints(ForceStepConstants.SaveBreakpointReason.ForceStep);
                dte.Debugger.StepOver(WaitForBreakOrEnd: false);
            }
        }


        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte = UtilityMethods.GetDTE(package);
            if (sender is OleMenuCommand menuCommand)
            {
                if (dte.Mode == EnvDTE.vsIDEMode.vsIDEModeDebug && dte.Debugger.CurrentMode == EnvDTE.dbgDebugMode.dbgBreakMode)
                {
                    menuCommand.Enabled = true;
                    menuCommand.Visible = true;
                }
                else
                {
                    menuCommand.Enabled = false;
                    menuCommand.Visible = false;
                }
            }
        }

    }
}
