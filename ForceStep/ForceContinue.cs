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
    internal sealed class ForceContinue
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4129;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("5499ab04-087c-4366-bd73-ff583b883993");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;


        private static bool IsFocused = false;


        /// <summary>
        /// Initializes a new instance of the <see cref="ForceContinue"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ForceContinue(AsyncPackage package, OleMenuCommandService commandService)
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
        public static ForceContinue Instance
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
            // Switch to the main thread - the call to AddCommand in ForceContinue's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ForceContinue(package, commandService);

            //if (await package.GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            //{
            //    // Create the command for the menu item.
            //    var menuCommandID1 = new CommandID(package., (int)PkgCmdIDList.FocusOnCurrentThreadCmd);
            //    var menuCommandID2 = new CommandID(GuidList.guidDebugSingleThreadCmdSet, (int)PkgCmdIDList.SwitchToNextThreadCmd);
            //    FocusCmd = new OleMenuCommand(FocusOnCurrentThread, menuCommandID1);
            //    SwitchCmd = new OleMenuCommand(SwitchToNextThread, menuCommandID2);
            //    FocusCmd.BeforeQueryStatus += OnBeforeQueryStatus;
            //    SwitchCmd.BeforeQueryStatus += OnBeforeQueryStatus;

            //    commandService.AddCommand(FocusCmd);
            //    commandService.AddCommand(SwitchCmd);

            //    dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
            //    OnBeforeQueryStatus(FocusCmd, null);
            //    OnBeforeQueryStatus(SwitchCmd, null);
            //}

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


            var bpm = new BreakpointManager(package);
            bpm.SaveAndSuspendActiveBreakpoints(ForceStepConstants.SaveBreakpointReason.ForceContinue);

            var dte = UtilityMethods.GetDTE(package);
            dte.Debugger.Go(WaitForBreakOrEnd: false);

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
