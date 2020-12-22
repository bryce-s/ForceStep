using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace ForceStep
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ForceStepInto
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4131;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("5499ab04-087c-4366-bd73-ff583b883993");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;


        public static DebuggerEvents m_debuggerEvents;
        private OleMenuCommand m_Command;


        /// <summary>
        /// Initializes a new instance of the <see cref="ForceStepInto"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ForceStepInto(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);

            var dte = UtilityMethods.GetDTE(package);
            m_debuggerEvents = dte.Events.DebuggerEvents;
            m_debuggerEvents.OnEnterBreakMode += OnEnterBreakMode;


            menuItem.BeforeQueryStatus += OnBeforeQueryStatus;

            m_Command = menuItem;

            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ForceStepInto Instance
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
            // Switch to the main thread - the call to AddCommand in ForceStepInto's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ForceStepInto(package, commandService);
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
                bpm.SaveAndSuspendActiveBreakpoints(ForceStepConstants.SaveBreakpointReason.ForceStepInto);
                dte.Debugger.StepInto(WaitForBreakOrEnd: false);
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

        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction executionAction)
        {
            m_Command.Enabled = true;
            m_Command.Visible = true;
        }

    }
}
