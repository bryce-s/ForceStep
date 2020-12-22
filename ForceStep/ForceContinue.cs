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
    internal sealed class ForceContinue
    {
        public const int CommandId = 4129;

        public static readonly Guid CommandSet = new Guid("5499ab04-087c-4366-bd73-ff583b883993");

        private readonly AsyncPackage package;

        public static DebuggerEvents m_debuggerEvents;
        private OleMenuCommand m_Command;

        private ForceContinue(AsyncPackage package, OleMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

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

        public static ForceContinue Instance
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
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ForceContinue(package, commandService);

        }

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


        private void OnEnterBreakMode(dbgEventReason reason, ref dbgExecutionAction executionAction)
        {
            m_Command.Enabled = true;
            m_Command.Visible = true;
        }



    }
}
